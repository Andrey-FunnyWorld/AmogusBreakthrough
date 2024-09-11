using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public Road Road;
    public RoadObjectsGenerator ObjectsGenerator;
    public RoadDataGenerator RoadDataGenerator;
    public MainGuy MainGuy;
    public LevelUIManager LevelUIManager;
    public StartGate StartGate, EndGate;
    [HideInInspector]
    public MovementController MovementController;
    public MovementController KeyboardController, TouchController;
    public PerkPanel PerkPanel;
    public TeamHealthController HealthController;
    public CoinsController CoinsController;
    public DebugLevelParams DebugLevelParams;
    public bool DEBUG; //TODO remove later

    public ImposterManager ImposterManager;
    public AudioSource BattleMusic, RoadWin;
    public float FadeBattleMusicDuration = 1.5f;
    public float MinBattleMusicVolume = 0.07f;
    const float END_GATE_OFFSET = 15;

    void Start() {
        SubscribeEvents();
        //Road.AssignRoadObjects(ObjectsGenerator.GetObjects(vm, Road.Length, Road.Width, roadTracksCoords));
        UserProgressController.Instance.ProgressState.ShowMenuOnStart = true;
        if (UserProgressController.ProgressLoaded)
            StartDataLoaded(null);
    }
    void OnDestroy() {
        UnsubscribeEvents();
    }
    void LetsRoll() {
        MainGuy.StartMove();
        Road.IsRunning = true;
        Road.MovementStarted = true;
        //LevelUIManager.LetsRoll();
        StartGate.Open();
        EndGate.GetComponent<RoadObjectBase>().RoadPosition = Road.Length + END_GATE_OFFSET;
        HtmlBridge.Instance.ReportMetric(MetricNames.BattleLevelStarted);
        BattleMusic.Play();
    }
    void StartMovement(object arg) {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        LetsRoll();
        if (DEBUG)
            EventManager.TriggerEvent(EventNames.MatesChanged);
    }
    void RoadFinished(object arg) {
        LevelUIManager.RoadFinished();
        StartCoroutine(FadeMusic(FadeBattleMusicDuration));
        RoadWin.Play();
        MovementController.AllowMove = false;
        MovementController.gameObject.SetActive(false);
        Road.MovementStarted = false;
        //LevelUIManager.RoadFinished();
        StartCoroutine(Utils.ChainActions(new List<ChainedAction>() {
            new ChainedAction() { DeltaTime = 1, Callback = () => { EndGate.Open(); }},
            new ChainedAction() { DeltaTime = 0.5f, Callback = () => { Road.IsRunning = true; }},
            new ChainedAction() { DeltaTime = 0.5f, Callback = () => { ImposterManager.RunImposterScene(); }},
            new ChainedAction() { DeltaTime = ImposterManager.ImposterUI.TransitionDuration,
                Callback = () => { Road.IsRunning = false; }},
        }));
    }
    IEnumerator FadeMusic(float time) {
        float timer = 0;
        float start = BattleMusic.volume;
        float end = MinBattleMusicVolume;
        while (timer < time) {
            timer += Time.deltaTime;
            BattleMusic.volume = end + (start - end) * (1 - timer / time);
            yield return null;
        }
        //BattleMusic.Stop();
    }
    void TeamDead(object arg0) {
        Road.IsRunning = false;
        Road.MovementStarted = false;
        MainGuy.TeamDead();
        LevelUIManager.ShowDefeatPanel(CoinsController.CasualCoins);
    }
    void HandleOnePunchAbility(object arg) {
        Road.AbilityOnePunchUsed();
    }
    void ApplyProgress(ProgressState progress) {
        MainGuy.ApplyProgress(progress);
        PerkPanel.ApplyProgress(progress);
        Road.PrepareAttackController();
        EventManager.TriggerEvent(EventNames.LevelLoaded, this);
        AdjustToPlatform(HtmlBridge.PlatformType);
        SetupRoadData();
    }
    void SetupRoadData() {
        int levelNumber = UserProgressController.Instance.ProgressState.CompletedRoundsCount;
        if (DebugLevelParams.UseDebugParams) {
            LevelLoader.Difficulty = DebugLevelParams.Difficulty;
            levelNumber = DebugLevelParams.LevelNumber;
        }
        Road.ZeroPointInWorld = MainGuy.transform.position.z;
        RoadDataGenerator.PivotEnemyHp = ObjectsGenerator.EnemySimplePrefab.StartHP;
        RoadDataGenerator.GiantHp = ObjectsGenerator.EnemyGiantPrefab.StartHP;
        RoadDataGenerator.GiantArmoredHp = ObjectsGenerator.EnemyArmoredGiantPrefab.StartHP;
        RoadDataViewModel vm = RoadDataGenerator.GetLevelViewModel(levelNumber, LevelLoader.Difficulty);
        Road.Length = vm.Length;
        Debug.Log(vm.ToString());
        Road.ViewModel = vm;
        ObjectsGenerator.ApplyProgress(levelNumber);
        Road.StartAction();
    }
    void AdjustToPlatform(PlatformType platformType) {
        StartGate.AdjustToPlatform(platformType);
        LevelUIManager.AdjustToPlatform(platformType);
    }
    void StartDataLoaded(object arg) {
        ApplyProgress(UserProgressController.Instance.ProgressState);
        MovementController = HtmlBridge.PlatformType == PlatformType.Desktop ? KeyboardController : TouchController;
        MovementController.gameObject.SetActive(true);
        MovementController.AllowMove = !PerkPanel.ShowOnStart;
    }
    void PerkSelected(object arg) {
        PerkItem perkItem = (PerkItem)arg;
        if (PerkPanel.ExtraPerkTaken)
            StartCoroutine(Utils.WaitAndDo(0.2f, () => {
                MovementController.AllowMove = true;
            }));
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

    public void StartGame() {
        StartCoroutine(Utils.WaitAndDo(0.2f, () => {
            MovementController.AllowMove = true;
        }));
        PerkPanel.gameObject.SetActive(false);
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

public interface IPlatformAdaptable {
    void Adapt(PlatformType platformType);
}
