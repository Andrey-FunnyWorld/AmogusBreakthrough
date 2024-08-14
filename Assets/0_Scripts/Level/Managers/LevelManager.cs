using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public Road Road;
    public RoadObjectsGenerator ObjectsGenerator;
    public MainGuy MainGuy;
    public LevelUIManager LevelUIManager;
    public StartGate StartGate, EndGate;
    [HideInInspector]
    public MovementController MovementController;
    public MovementController KeyboardController, TouchController;
    public PerkPanel PerkPanel;
    public ImposterManager ImposterManager;
    public AudioSource BattleMusic, RoadWin;
    public float FadeBattleMusicDuration = 1.5f;
    const float END_GATE_OFFSET = 15;
    void Start() {
        SubscriveEvents();
        Road.ZeroPointInWorld = MainGuy.transform.position.z;
        List<float> roadTracksCoords = Road.InitTracks();
        Road.AssignRoadObjects(ObjectsGenerator.GetObjects(0, Road.Length, Road.Width, roadTracksCoords, 0));
        UserProgressController.Instance.ProgressState.ShowMenuOnStart = true;
        if (UserProgressController.ProgressLoaded)
            StartDataLoaded(null);
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
    void LetsRoll() {
        Road.PrepareAttackController();
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
    }
    void RoadFinished(object arg) {
        //LevelUIManager.RoadFinished();
        StartCoroutine(FadeMusic(FadeBattleMusicDuration));
        RoadWin.Play();
        MovementController.AllowMove = false;
        MovementController.gameObject.SetActive(false);
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
        while (timer < time) {
            timer += Time.deltaTime;
            BattleMusic.volume = start * (1 - timer / time);
            yield return null;
        }
        BattleMusic.Stop();
    }
    void ApplyProgress(ProgressState progress) {
        MainGuy.ApplyProgress(progress);
        PerkPanel.ApplyProgress(progress);
        EventManager.TriggerEvent(EventNames.LevelLoaded, this);
        AdjustToPlatform(HtmlBridge.PlatformType);
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
        Debug.Log("TO-DO. Apply Perk: " + perkItem.PerkType);
    }
    public void StartGame() {
        StartCoroutine(Utils.WaitAndDo(0.2f, () => {
            MovementController.AllowMove = true;
        }));
        PerkPanel.gameObject.SetActive(false);
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.StartMovement, StartMovement);
        EventManager.StartListening(EventNames.RoadFinished, RoadFinished);
        #if UNITY_EDITOR
        EventManager.StartListening(EventNames.StartDataLoaded, StartDataLoaded);
        #endif
        EventManager.StartListening(EventNames.PerkSelected, PerkSelected);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        EventManager.StopListening(EventNames.RoadFinished, RoadFinished);
        #if UNITY_EDITOR
        EventManager.StopListening(EventNames.StartDataLoaded, StartDataLoaded);
        #endif
        EventManager.StopListening(EventNames.PerkSelected, PerkSelected);
    }
}
