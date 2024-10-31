using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFovAutoSize : BaseAutoSizer {
    public CinemachineVirtualCamera Camera;
    public float WidthTreshold = 880;
    public float DefaultFov = 50;
    protected override void Adjust(int width, int height) {
        //Debug.LogFormat("{0}x{1}", width, height);
        if (width < WidthTreshold) {
            float fov = -0.115f * width + 151.2f;
            Camera.m_Lens.FieldOfView = fov;
        } else {
            Camera.m_Lens.FieldOfView = DefaultFov;
        }
    }
}
