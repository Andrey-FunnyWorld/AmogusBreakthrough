using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public Road Road;
    public RoadObjectsGenerator ObjectsGenerator;
    public MainGuy MainGuy;
    public LevelUIManager LevelUIManager;
    public MovementController MovementController;
    public PerkPanel PerkPanel;
    public TeamHealthController HealthController;
    public CoinsController CoinsController;

    void Start() {
        SubscribeEvents();
        Road.ZeroPointInWorld = MainGuy.transform.position.z;
        List<float> roadTracksCoords = Road.InitTracks();
        Road.AssignRoadObjects(ObjectsGenerator.GetObjects(0, Road.Length, Road.Width, roadTracksCoords, 0));
        MovementController.AllowMove = !PerkPanel.ShowOnStart;
        // ApplyProgress(UserProgressController.Instance.ProgressState);
        EventManager.TriggerEvent(EventNames.LevelLoaded, this);
    }
    void Update() {
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     Road.IsRunning = !Road.IsRunning;
        // }
    }
    void OnDestroy() {
        UnsubscribeEvents();
    }
    void LetsRoll() {
        Road.PrepareAttackController();
        MainGuy.StartMove();
        Road.IsRunning = true;
        Road.MovementStarted = true;
        LevelUIManager.LetsRoll();
    }
    void StartMovement(object arg) {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        LetsRoll();
    }
    void RoadFinished(object arg) {
        Road.MovementStarted = false;
        LevelUIManager.RoadFinished();
    }
    void TeamDead(object arg0) {
        //TODO game over
    }
    void ApplyProgress(ProgressState progress) {
        MainGuy.ApplyProgress(progress);
        EventManager.TriggerEvent(EventNames.LevelLoaded, this);
    }
    void StartDataLoaded(object arg) {
        ApplyProgress(UserProgressController.Instance.ProgressState);
    }
    void PerkSelected(object arg) {
        PerkItem perkItem = (PerkItem)arg;
        MovementController.AllowMove = true;
        Debug.Log($"TO-DO. Apply Perk: {perkItem.PerkType}");
        HandlePerk(perkItem);
    }
    void HandlePerk(PerkItem perkItem) {
        PerkType perk = perkItem.PerkType;
        if (perk == PerkType.SlowWalkSpeed) {
            Road.ApplySlowerMoveSpeedPerk();
        } else if (perk == PerkType.BossDamage || perk == PerkType.ExtraAttackWidth || perk == PerkType.AttackZoneVisibility) {
            Road.HandlePerk(perk);
        } else if (perk == PerkType.ExtraHealth || perk == PerkType.ExtraHealthUltra || perk == PerkType.RegenHP) {
            HealthController.HandlePerk(perk);
        } else if (perk == PerkType.WeaponBoxTransparency) {
            ObjectsGenerator.HandleWeaponBoxTransparencyPerk();
        } else if (perk == PerkType.OnePunchKill || perk == PerkType.Bubble) {
            LevelUIManager.HandlePerk(perk);
        } else if (perk == PerkType.ExtraCoins) {
            CoinsController.ApplyExtraCoinsPerk();
        } else if (perk == PerkType.ExtraGuy) {
            MainGuy.ApplyExtraGuyPerk(perk);
        }
    }
    void SubscribeEvents() {
        EventManager.StartListening(EventNames.StartMovement, StartMovement);
        EventManager.StartListening(EventNames.RoadFinished, RoadFinished);
        #if UNITY_EDITOR
        EventManager.StartListening(EventNames.StartDataLoaded, StartDataLoaded);
        #endif
        EventManager.StartListening(EventNames.PerkSelected, PerkSelected);
        EventManager.StartListening(EventNames.TeamDead, TeamDead);
    }

    void UnsubscribeEvents() {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        EventManager.StopListening(EventNames.RoadFinished, RoadFinished);
        #if UNITY_EDITOR
        EventManager.StopListening(EventNames.StartDataLoaded, StartDataLoaded);
        #endif
        EventManager.StopListening(EventNames.PerkSelected, PerkSelected);
        EventManager.StopListening(EventNames.TeamDead, TeamDead);
    }
}
