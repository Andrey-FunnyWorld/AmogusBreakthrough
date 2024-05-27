using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public Road Road;
    public RoadObjectsGenerator ObjectsGenerator;
    public MainGuy MainGuy;
    public LevelUIManager LevelUIManager;
    public MovementController MovementController;
    public PerkPanel PerkPanel;
    void Start() {
        SubscriveEvents();
        Road.ZeroPointInWorld = MainGuy.transform.position.z;
        Road.AssignRoadObjects(ObjectsGenerator.GetObjects(0, Road.Length, Road.Width, 0));
        MovementController.AllowMove = !PerkPanel.ShowOnStart;
    }
    void Update() {
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     Road.IsRunning = !Road.IsRunning;
        // }
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
    void LetsRoll() {
        MainGuy.StartMove();
        Road.IsRunning = true;
        LevelUIManager.LetsRoll();
    }
    void StartMovement(object arg) {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        LetsRoll();
    }
    void RoadFinished(object arg) {
        LevelUIManager.RoadFinished();
    }
    void ApplyProgress(ProgressState progress) {
        MainGuy.ApplyProgress(progress);
    }
    void StartDataLoaded(object arg) {
        ApplyProgress(UserProgressController.Instance.ProgressState);
    }
    void PerkSelected(object arg) {
        PerkItem perkItem = (PerkItem)arg;
        MovementController.AllowMove = true;
        Debug.Log("TO-DO. Apply Perk: " + perkItem.PerkType);
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.StartMovement, StartMovement);
        EventManager.StartListening(EventNames.RoadFinished, RoadFinished);
        EventManager.StartListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StartListening(EventNames.PerkSelected, PerkSelected);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        EventManager.StopListening(EventNames.RoadFinished, RoadFinished);
        EventManager.StopListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StopListening(EventNames.PerkSelected, PerkSelected);
    }
}
