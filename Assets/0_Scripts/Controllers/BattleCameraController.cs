using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BattleCameraController : MonoBehaviour {
    public CinemachineVirtualCamera Battle, ImposterOverview, Platform, ImposterEnd;
    public CinemachineBrain Brain;
    BattleCameraType currentCamera;
    CinemachineVirtualCamera[] cameras;
    void Start() {
        cameras = new CinemachineVirtualCamera[4] { Battle, ImposterOverview, Platform, ImposterEnd };
    }
    public void SwitchToCamera(int cameraType) {
        currentCamera = (BattleCameraType)cameraType;
        CinemachineVirtualCamera targetCam;
        switch ((BattleCameraType)cameraType) {
            case BattleCameraType.Imposter: targetCam = ImposterOverview; break;
            case BattleCameraType.Platform: targetCam = Platform; break;
            case BattleCameraType.ImposterEnd: targetCam = ImposterEnd; break;
            default: targetCam = Battle; break;
        }
        currentCamera = (BattleCameraType)cameraType;
        foreach (CinemachineVirtualCamera cam in cameras) {
            cam.gameObject.SetActive(cam == targetCam);
        }
    }
    public void SwitchToCamera(BattleCameraType nextCamera) {
        SwitchToCamera((int)nextCamera);
    }
}
public enum BattleCameraType {
    Battle, Imposter, Platform, ImposterEnd
}