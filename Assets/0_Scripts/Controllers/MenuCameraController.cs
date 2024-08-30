using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MenuCameraController : MonoBehaviour {
    public CinemachineVirtualCamera FaceCamera, BackpackFarCamera, BackpackCloseCamera, HatCloseCamera, HatFarCamera,
        BackpackMedium, HatMedium, Desintegrator, DesintegratorScene;
    CameraType currentCamera;
    CinemachineVirtualCamera[] cameras;
    void Start() {
        cameras = new CinemachineVirtualCamera[8] { FaceCamera, BackpackFarCamera, BackpackCloseCamera, HatCloseCamera, HatFarCamera, BackpackMedium, Desintegrator, DesintegratorScene };
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
            case CameraType.BackpackMedium: targetCam = BackpackMedium; break;
            case CameraType.Desintegrator: targetCam = Desintegrator; break;
            case CameraType.DesintegratorScene: targetCam = DesintegratorScene; break;
            //case CameraType.HatMedium: targetCam = BackpackMedium; break;
            default: targetCam = FaceCamera; break;
        }
        currentCamera = (CameraType)cameraType;
        foreach (CinemachineVirtualCamera cam in cameras) {
            cam.gameObject.SetActive(cam == targetCam);
        }
    }
    public void SwitchToCamera(CameraType nextCamera) {
        SwitchToCamera((int)nextCamera);
    }
    // public void ToogleBackpackCameras() {
    //     CameraType cameraType = currentCamera == CameraType.BackpackFar ? CameraType.BackpackClose : CameraType.BackpackFar;
    //     SwitchToCamera((int)cameraType);
    // }
    // public void ToogleHatCameras() {
    //     CameraType cameraType = currentCamera == CameraType.HatFar ? CameraType.HatClose : CameraType.HatFar;
    //     SwitchToCamera((int)cameraType);
    // }
}
public enum CameraType {
    Face, BackpackFar, BackpackClose, HatClose, HatFar, BackpackMedium, Desintegrator, DesintegratorScene
}