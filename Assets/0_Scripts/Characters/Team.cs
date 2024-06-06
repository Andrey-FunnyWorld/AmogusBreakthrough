using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour
{
    public MainGuy MainGuy;
    public Amogus AmogusPrefab;
    public TeamLayout Layout = TeamLayout.Battle;
    public HatNamePrefabMap HatNamePrefabMap;
    public float AmogusDamage = 0.1f;
    [HideInInspector] public int Count;
    public int StartupCount = 3;
    List<Color> colors;
    List<Amogus> Mates;
    MyRange teamRange = new MyRange();

    public MyRange TeamRange { get { return teamRange; } }
    public int MatesCount => Mates.Count;

    public float AttackRange = 10f;
    [NonSerialized] public Transform MostLeftMate;
    [NonSerialized] public Transform MostRightMate;

    const float X_OFFSET = 0.6f;
    const float Z_OFFSET = 0.5f;
    const float MIN_RANGE = 0.4f;
    public const int MAX_CAPACITY = 10;

    void Start() {
        SubscriveEvents();
    }

    void OnDestroy() {
        UnsubscriveEvents();
    }

    public void CreateTeam(ProgressState state) {
        Mates = new List<Amogus>(StartupCount);
        colors = new List<Color>(StartupCount);
        for (int i = 0; i < StartupCount; i++)
        {
            CreateMate((SkinItemName)state.EquippedBackpacks[i + 1], (SkinItemName)state.EquippedHats[i + 1]);
        }
        CalcTeamRange();
    }
    public void AddNewMate() {
        SkinItemName skinBackpack = (SkinItemName)UserProgressController.Instance.ProgressState.EquippedBackpacks[Count];
        SkinItemName skinHat = (SkinItemName)UserProgressController.Instance.ProgressState.EquippedHats[Count];
        CreateMate(skinBackpack, skinHat);
        CalcTeamRange();
    }
    void CreateMate(SkinItemName backpackSkin, SkinItemName hatSkin) {
        Amogus newMate = Instantiate(AmogusPrefab, transform);
        Vector3 offset = CalcOffset();
        newMate.PositionOffset = offset;
        newMate.transform.Translate(offset + new Vector3(MainGuy.transform.position.x, 0, 0));
        ApplyMaterials(newMate, backpackSkin);
        ApplyHat(newMate, hatSkin);
        Mates.Add(newMate);

        if (MostLeftMate == null || MostLeftMate.position.x > offset.x)
        {
            MostLeftMate = newMate.transform;
        }
        if (MostRightMate == null || MostRightMate.position.x < offset.x)
        {
            MostRightMate = newMate.transform;
        }
    }
    void ApplyMaterials(Amogus mate, SkinItemName backpackSkin) {
        colors.Add(GetNextColor());
        Material colorMaterial = MaterialStorage.Instance.GetColorMaterial(colors.Last());
        Material skinMaterial = MaterialStorage.Instance.GetBackpackMaterial(backpackSkin);
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
        }
        else if (Layout == TeamLayout.Line) {
            offset = new Vector3(
                2 * X_OFFSET * (Mates.Count / 2 + 1) * (Mates.Count % 2 == 0 ? -1 : 1),
                0, 0
            );
        }
        return offset;
    }
    void CalcTeamRange() {
        teamRange.Start = Mathf.Min(-MIN_RANGE, -X_OFFSET * ((Mates.Count + 1) / 2));
        teamRange.End = Mathf.Max(MIN_RANGE, X_OFFSET * (Mates.Count / 2));
    }
    public void ApplyMovement(Vector3 newPosition) {
        foreach (Amogus amogus in Mates) {
            amogus.ApplyMovement(newPosition);
        }
    }
    Color GetNextColor() {
        return TeamColors[colors.Count];
        // List<Color> availableColors = TeamColors.Except(colors).ToList();
        // return availableColors[Random.Range(0, availableColors.Count)];
    }
    static Color[] TeamColors = new Color[10] {
        Color.red, Color.blue, Color.green, Color.magenta, Color.yellow, Color.grey,
        new Color(1, 0.49f, 0.19f), new Color(0.54f, 0.17f, 0.88f), new Color(0.57f, 0.44f, 0.86f), new Color(0.5f, 0.5f,0)
    };

    #region Events
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.CageDestroyed, CageDestroyed);
        EventManager.StartListening(EventNames.RandomSkins, ApplyRandomSkins);
        EventManager.StartListening(EventNames.SkinItemEquip, SkinItemEquip);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.CageDestroyed, CageDestroyed);
        EventManager.StopListening(EventNames.RandomSkins, ApplyRandomSkins);
        EventManager.StopListening(EventNames.SkinItemEquip, SkinItemEquip);
    }
    void ApplyRandomSkins(object arg) {
        RandomSkinArg skinArg = (RandomSkinArg)arg;
        for (int i = 0; i < Mates.Count; i++) {
            if (skinArg.ShopType == ShopType.Backpack) {
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
            if (args.ShopType == ShopType.Backpack) {
                Material skinMaterial = args.ItemModel.SkinName != SkinItemName.None
                    ? MaterialStorage.Instance.GetBackpackMaterial(args.ItemModel.SkinName):
                    null;
                Mates[i].ApplyBackpack(skinMaterial);
            } else {
                ApplyHat(Mates[i], args.ItemModel.SkinName);
            }
        }
    }
    void CageDestroyed(object arg) {
        if (Count == MAX_CAPACITY) Debug.LogError("Cage Destroyed: TEAM ALREADY FULL");
        else {
            Cage cage = (Cage)arg;
            AddNewMate();
        }
    }
    #endregion

    void OnDrawGizmos() {
        if (MostLeftMate != null) {
            Gizmos.color = new Color(1, 1, 0, 0.25F);
            Gizmos.DrawCube(GetTeamPosition(), GetTeamSize());
        }
    }
    private Vector3 GetTeamPosition() {
        return new Vector3(
            (MostLeftMate.position.x + MostRightMate.position.x) / 2,
            2,
            transform.position.z + AttackRange / 2 + 1
        );
    }
    private Vector3 GetTeamSize() {
        return new Vector3(Mathf.Abs(teamRange.Start) + Mathf.Abs(teamRange.End), 4, AttackRange);
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