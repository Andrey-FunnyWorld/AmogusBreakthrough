using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public Road Road;
    public RoadObjectsGenerator ObjectsGenerator;
    public MainGuy MainGuy;
    public LevelUIManager LevelUIManager;
    public StartGate StartGate, EndGate;
    public MovementController MovementController;
    public PerkPanel PerkPanel;
    public TeamHealthController HealthController;
    public CoinsController CoinsController;
    public bool DEBUG; //TODO remove later

    public ImposterManager ImposterManager;
    const float END_GATE_OFFSET = 15;

    void Start() {
        SubscribeEvents();
        //Road.PrepareAttackController();
        Road.ZeroPointInWorld = MainGuy.transform.position.z;
        List<float> roadTracksCoords = Road.InitTracks();
        Road.AssignRoadObjects(ObjectsGenerator.GetObjects(0, Road.Length, Road.Width, roadTracksCoords, 0));
        MovementController.AllowMove = !PerkPanel.ShowOnStart;
        // ApplyProgress(UserProgressController.Instance.ProgressState);
        EventManager.TriggerEvent(EventNames.LevelLoaded, this);
    }

    void OnDestroy() {
        UnsubscribeEvents();
    }
    void LetsRoll() {
        MainGuy.StartMove();
        Road.IsRunning = true;
        Road.MovementStarted = true;
        LevelUIManager.LetsRoll();
        StartGate.Open();
        EndGate.GetComponent<RoadObjectBase>().RoadPosition = Road.Length + END_GATE_OFFSET;
    }
    void StartMovement(object arg) {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        LetsRoll();
        if (DEBUG)
            EventManager.TriggerEvent(EventNames.MatesChanged);
    }
    void RoadFinished(object arg) {
        Road.MovementStarted = false;
        LevelUIManager.RoadFinished();
        StartCoroutine(Utils.ChainActions(new List<ChainedAction>() {
            new ChainedAction() { DeltaTime = 1, Callback = () => { EndGate.Open(); }},
            new ChainedAction() { DeltaTime = 0.5f, Callback = () => { Road.IsRunning = true; }},
            new ChainedAction() { DeltaTime = 0.5f, Callback = () => { ImposterManager.RunImposterScene(); }},
            new ChainedAction() { DeltaTime = ImposterManager.ImposterUI.TransitionDuration,
                Callback = () => { Road.IsRunning = false; }},
        }));
    }
    void TeamDead(object arg0) {
        //TODO game over
    }
    void HandleOnePunchAbility(object arg) {
        Road.AbilityOnePunchUsed();
    }
    void ApplyProgress(ProgressState progress) {
        MainGuy.ApplyProgress(progress);
        Road.PrepareAttackController();
        EventManager.TriggerEvent(EventNames.LevelLoaded, this);
    }
    void StartDataLoaded(object arg) {
        ApplyProgress(UserProgressController.Instance.ProgressState);
    }
    void PerkSelected(object arg) {
        PerkItem perkItem = (PerkItem)arg;
        MovementController.AllowMove = true;
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
            Road.HandleTransparencyWeaponBoxPerk();
        } else if (perk == PerkType.OnePunchKill || perk == PerkType.Bubble) {
            LevelUIManager.HandlePerk(perkItem);
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
        EventManager.StartListening(EventNames.AbilityOnePunch, HandleOnePunchAbility);
    }

    void UnsubscribeEvents() {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        EventManager.StopListening(EventNames.RoadFinished, RoadFinished);
        #if UNITY_EDITOR
        EventManager.StopListening(EventNames.StartDataLoaded, StartDataLoaded);
        #endif
        EventManager.StopListening(EventNames.PerkSelected, PerkSelected);
        EventManager.StopListening(EventNames.TeamDead, TeamDead);
        EventManager.StopListening(EventNames.AbilityOnePunch, HandleOnePunchAbility);
    }
}
