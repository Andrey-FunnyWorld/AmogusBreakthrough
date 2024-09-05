using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour {
    public MainGuy MainGuy;
    public Amogus AmogusPrefab;
    public TeamLayout Layout = TeamLayout.Battle;
    public HatNamePrefabMap HatNamePrefabMap;
    public float AmogusDamage = 0.1f;
    public int StartupCount = 3;
    public TeamHealthController TeamHealth;
    List<Color> colors;
    List<Amogus> Mates;
    MyRange teamRange = new MyRange();

    public MyRange TeamRange { get { return teamRange; } }
    public int MatesCount => Mates.Count;

    public float AttackRange = 10f;
    public Transform MostLeftMate;
    public Transform MostRightMate;
    float leftMateHalfSize;
    float rightMateHalfSize;

    const float X_OFFSET_LINE = 1.5f;
    const float X_OFFSET = 0.6f;
    const float Z_OFFSET = 0.7f;
    const float MIN_RANGE = 0.4f;
    public const int MAX_CAPACITY = 10;

    void Start() => SubscribeEvents();
    void OnDestroy() => UnsubscribeEvents();

    public Amogus GetMate(int index) {
        return Mates[index];
    }

    public void CreateTeam(ProgressState state) {
        Mates = new List<Amogus>(StartupCount);
        colors = new List<Color>(StartupCount);
        for (int i = 0; i < StartupCount; i++) {
            CreateMate((SkinItemName)state.EquippedBackpacks[i + 1], (SkinItemName)state.EquippedHats[i + 1]);
        }
        CalcTeamRange();
    }
    public void AddNewMate(bool isRunning = false) {
        SkinItemName skinBackpack = (SkinItemName)UserProgressController.Instance.ProgressState.EquippedBackpacks[MatesCount];
        SkinItemName skinHat = (SkinItemName)UserProgressController.Instance.ProgressState.EquippedHats[MatesCount];
        CreateMate(skinBackpack, skinHat, isRunning);
        CalcTeamRange();
        EventManager.TriggerEvent(EventNames.MatesChanged);
    }
    public void ApplyMovement(Vector3 newPosition) {
        foreach (Amogus amogus in Mates) {
            amogus.ApplyMovement(newPosition);
        }
    }
    public void ApplyExtraGuyPerk(PerkType perk) {
        if (perk == PerkType.ExtraGuy) {
            if (MatesCount == MAX_CAPACITY)
                Debug.LogError("Perk Extra Guy: TEAM ALREADY FULL");
            else
                AddNewMate();
        }
    }
    public void SwitchWeapon(WeaponType weaponType) {
        foreach (var mate in Mates)
            mate.SwitchWeapon(weaponType);
    }

    void CreateMate(SkinItemName backpackSkin, SkinItemName hatSkin, bool isRunning = false) {
        Amogus newMate = Instantiate(AmogusPrefab, transform);
        Vector3 offset = CalcOffset();
        newMate.SetGun(Mates.Count % 2 == 0);
        newMate.PositionOffset = offset;
        newMate.transform.Translate(offset + new Vector3(MainGuy.transform.position.x, 0, 0));
        ApplyMaterials(newMate, backpackSkin);
        ApplyHat(newMate, hatSkin);
        Mates.Add(newMate);
        newMate.SetRun(isRunning);
        if (MostLeftMate == null || MostLeftMate.position.x > newMate.transform.position.x) {
            MostLeftMate = newMate.transform;
            leftMateHalfSize = GetHalfScaleX(MostLeftMate);
        }
        
        if (MostRightMate == null || MostRightMate.position.x < newMate.transform.position.x) {
            MostRightMate = newMate.transform;
            rightMateHalfSize = GetHalfScaleX(MostRightMate);
        }
        Debug.Log("Team Width: " + (MostRightMate.position.x - MostLeftMate.position.x + leftMateHalfSize + rightMateHalfSize));
    }
    void ApplyMaterials(Amogus mate, SkinItemName backpackSkin) {
        colors.Add(GetNextColor());
        Material colorMaterial = MaterialStorage.Instance.GetColorMaterial(colors.Last());
        Material skinMaterial = backpackSkin != SkinItemName.None ? MaterialStorage.Instance.GetBackpackMaterial(backpackSkin) : null;
        mate.ApplyMaterials(colorMaterial, skinMaterial);
    }
    void ApplyHat(Amogus mate, SkinItemName hatSkin) {
        if (mate.ActiveHat != null)
            HatStorage.Instance.RemoveHat(mate.ActiveHat);
        if (hatSkin != SkinItemName.None) {
            mate.ApplyHat(HatStorage.Instance.GetHat(hatSkin));
        } else {
            mate.ApplyHat(null);
        }
    }
    Vector3 CalcOffset() {
        Vector3 offset = Vector3.zero;
        if (Layout == TeamLayout.Battle) {
            offset = new Vector3(
                X_OFFSET * (Mates.Count / 2 + 1) * (Mates.Count % 2 == 0 ? -1 : 1),
                0,
                Z_OFFSET * ((Mates.Count / 2) % 2 == 0 ? -1 : 1)
            );
        } else if (Layout == TeamLayout.Line) {
            offset = new Vector3(
                X_OFFSET_LINE * (Mates.Count / 2 + 1) * (Mates.Count % 2 == 0 ? -1 : 1),
                0, 0
            );
        }
        return offset;
    }
    void CalcTeamRange() {
        teamRange.Start = Mathf.Min(-MIN_RANGE, -X_OFFSET * ((Mates.Count + 1) / 2));
        teamRange.End = Mathf.Max(MIN_RANGE, X_OFFSET * (Mates.Count / 2));
    }
    Color GetNextColor() {
        return TeamColors[colors.Count];
        // List<Color> availableColors = TeamColors.Except(colors).ToList();
        // return availableColors[Random.Range(0, availableColors.Count)];
    }
    public static Color[] TeamColors = new Color[10] {
        Color.red, Color.blue, Color.green, Color.magenta, Color.yellow, Color.grey,
        new Color(1, 0.49f, 0.19f), new Color(0.54f, 0.17f, 0.88f), new Color(0.57f, 0.44f, 0.86f), new Color(0.5f, 0.5f,0)
    };

    #region Events
    void SubscribeEvents() {
        EventManager.StartListening(EventNames.CageDestroyed, CageDestroyed);
        EventManager.StartListening(EventNames.RandomSkins, ApplyRandomSkins);
        EventManager.StartListening(EventNames.SkinItemEquip, SkinItemEquip);
        EventManager.StartListening(EventNames.StartMovement, StartMovement);
        EventManager.StartListening(EventNames.RoadFinished, RoadFinished);
    }
    void UnsubscribeEvents() {
        EventManager.StopListening(EventNames.CageDestroyed, CageDestroyed);
        EventManager.StopListening(EventNames.RandomSkins, ApplyRandomSkins);
        EventManager.StopListening(EventNames.SkinItemEquip, SkinItemEquip);
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        EventManager.StopListening(EventNames.RoadFinished, RoadFinished);
    }

    void StartMovement(object arg) {
        MainGuy.SetRun(true);
        foreach (Amogus mate in Mates)
            mate.SetRun(true);
    }
    void RoadFinished(object arg) {
        MainGuy.SetRun(false);
        foreach (Amogus mate in Mates)
            mate.SetRun(false);
    }
    void ApplyRandomSkins(object arg) {
        RandomSkinArg skinArg = (RandomSkinArg)arg;
        for (int i = 0; i < Mates.Count; i++) {
            if (skinArg.ShopType == SkinType.Backpack) {
                Material skinMaterial = MaterialStorage.Instance.GetBackpackMaterial((SkinItemName)skinArg.RandomSkins[i]);
                Mates[i].ApplyBackpack(skinMaterial);
            } else {
                ApplyHat(Mates[i], (SkinItemName)skinArg.RandomSkins[i]);
            }
        }
    }
    void SkinItemEquip(object arg) {
        SkinItemEquipArgs args = (SkinItemEquipArgs)arg;
        for (int i = 0; i < Mates.Count; i++) {
            if (args.ShopType == SkinType.Backpack) {
                Material skinMaterial = args.ItemModel.SkinName != SkinItemName.None
                    ? MaterialStorage.Instance.GetBackpackMaterial(args.ItemModel.SkinName)
                    : null;
                Mates[i].ApplyBackpack(skinMaterial);
            } else {
                ApplyHat(Mates[i], args.ItemModel.SkinName);
            }
        }
    }
    void CageDestroyed(object arg) {
        if (MatesCount == MAX_CAPACITY) Debug.LogError("Cage Destroyed: TEAM ALREADY FULL");
        else {
            Cage cage = (Cage)arg;
            AddNewMate(true);
        }
    }
    #endregion

    float GetHalfScaleX(Transform transform) {
        return transform.localScale.x / 2;
    }

    void OnDrawGizmos() {
        if (MostLeftMate != null) {
            Gizmos.color = new Color(1, 1, 0, 0.15F);
            Gizmos.DrawCube(GetTeamPosition(), GetTeamSize());
        }
    }
    public Vector3 GetTeamPosition() {
        return new Vector3(
            (MostLeftMate.position.x + MostRightMate.position.x) / 2,
            0,
            transform.position.z + AttackRange / 2 + 1
        );
    }
    public Vector3 GetTeamSize(bool extraAttackWidth = false) {
        float extraWidth = extraAttackWidth ? 2f : 0f;
        return new Vector3(
            MostRightMate.position.x - MostLeftMate.position.x + extraWidth + leftMateHalfSize + rightMateHalfSize,
            1,
            AttackRange
        );
    }
    public void HideGuns() {
        foreach (Amogus mate in Mates) {
            mate.GunPlaceholderLeft.DeactivateChildren();
            mate.GunPlaceholderRight.DeactivateChildren();
        }
    }
}

public class MyRange {
    public float Start;
    public float End;
    public override string ToString() {
        return string.Format("Start: {0} | End: {1}", Start, End);
    }
}

public enum TeamLayout {
    Battle, Line
}