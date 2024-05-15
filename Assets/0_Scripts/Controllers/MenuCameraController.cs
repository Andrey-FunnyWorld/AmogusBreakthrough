using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MenuCameraController : MonoBehaviour {
    public CinemachineVirtualCamera FaceCamera, BackpackFarCamera, BackpackCloseCamera, HatCloseCamera, HatFarCamera;
    CameraType currentCamera;
    CinemachineVirtualCamera[] cameras;
    void Start() {
        cameras = new CinemachineVirtualCamera[5] { FaceCamera, BackpackFarCamera, BackpackCloseCamera, HatCloseCamera, HatFarCamera };
    }
    public void SwitchToCamera(int cameraType) {
        currentCamera = (CameraType)cameraType;
        CinemachineVirtualCamera targetCam = FaceCamera;
        switch ((CameraType)cameraType) {
            case CameraType.Face: targetCam = FaceCamera; break;
            case CameraType.BackpackFar: targetCam = BackpackFarCamera; break;
            case CameraType.BackpackClose: targetCam = BackpackCloseCamera; break;
            case CameraType.HatFar: targetCam = HatFarCamera; break;
            case CameraType.HatClose: targetCam = HatCloseCamera; break;
            default: targetCam = FaceCamera; break;
        }
        foreach (CinemachineVirtualCamera cam in cameras) {
            cam.gameObject.SetActive(cam == targetCam);
        }
    }
    public void ToogleBackpackCameras() {
        CameraType cameraType = currentCamera == CameraType.BackpackFar ? CameraType.BackpackClose : CameraType.BackpackFar;
        SwitchToCamera((int)cameraType);
    }
    public void ToogleHatCameras() {
        CameraType cameraType = currentCamera == CameraType.HatFar ? CameraType.HatClose : CameraType.HatFar;
        SwitchToCamera((int)cameraType);
    }
}
public enum CameraType {
    Face, BackpackFar, BackpackClose, HatClose, HatFar
}