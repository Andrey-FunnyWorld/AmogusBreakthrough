using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public Road Road;
    public RoadObjectsGenerator ObjectsGenerator;
    public MainGuy MainGuy;
    public LevelUIManager LevelUIManager;

    void Start() {
        SubscriveEvents();
        Road.ZeroPointInWorld = MainGuy.transform.position.z;
        List<float> roadTracksCoords = Road.InitTracks();
        Road.AssignRoadObjects(ObjectsGenerator.GetObjects(0, Road.Length, Road.Width, roadTracksCoords, 0));
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
        Road.MovementStarted = true;
        LevelUIManager.LetsRoll();
    }
    void StartMovement(object arg) {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        LetsRoll();
    }
    void RoadFinished(object arg) {
        LevelUIManager.RoadFinished();
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.StartMovement, StartMovement);
        EventManager.StartListening(EventNames.RoadFinished, RoadFinished);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        EventManager.StopListening(EventNames.RoadFinished, RoadFinished);
    }
}
