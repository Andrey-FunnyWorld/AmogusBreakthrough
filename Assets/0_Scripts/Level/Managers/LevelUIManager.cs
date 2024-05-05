using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUIManager : MonoBehaviour {
    public Transform StartupMsg;
    public Transform RoadFinishedMsg;
    public void LetsRoll() {
        StartupMsg.gameObject.SetActive(false);
    }
    public void RoadFinished() {
        RoadFinishedMsg.gameObject.SetActive(true);
    }
}
