using System;
using UnityEngine;

public class RunningTexture : MonoBehaviour {
    public MeshRenderer Renderer;
    public bool AnimateX;
    public bool AnimateForward;
    public Axis AnimationAxis;
    [NonSerialized]
    public bool IsRunning = false;
    float moveTime = 0;
    float texOffsetFactor = 0;
    float speed = 0;

    void Update() {
        if (IsRunning) {
            moveTime += Time.deltaTime;
            float texOffset = moveTime / texOffsetFactor;
            float x = AnimateX ? (-texOffset % 1) * (AnimateForward ? 1 : -1) : 0;
            float y = AnimateX ? 0 : (-texOffset % 1) * (AnimateForward ? 1 : -1);
            Renderer.material.mainTextureOffset = new Vector2(x, y);
        }
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
        float texScale = AnimateX ? Renderer.material.mainTextureScale.x : Renderer.material.mainTextureScale.y;
        float transformScale = 0;
        switch (AnimationAxis) {
            case Axis.X: transformScale = transform.localScale.x; break;
            case Axis.Y: transformScale = transform.localScale.y; break;
            case Axis.Z: transformScale = transform.localScale.z; break;
        }
        texOffsetFactor = 10 / ((texScale / transformScale) * speed);
    }
}

public enum Axis {
    X, Y, Z
}