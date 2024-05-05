using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour {
    public float StartSpeed = 2;
    public MeshRenderer RoadMeshRenderer;
    [HideInInspector]
    public float ZeroPointInWorld;

    void Start() {
        Speed = StartSpeed;
    }

    public bool IsRunning { get { return isRunning; }
        set {
            if (isRunning != value) {
                isRunning = value;
                // toggle environment activity if necessary
            }
        }
    }
    float speed = 0;
    public float Speed {
        get { return speed; }
        set {
            if (speed != value) {
                speed = value;
                texOffsetFactor = 10 / ((RoadMeshRenderer.material.mainTextureScale.y / transform.localScale.z) * Speed);
            }
        }
    }
    float texOffsetFactor = 0;
    bool isRunning = false;
    float moveTime = 0;
    float currentPosition = 0;
    List<RoadObjectBase> roadObjects;
    void Update() {
        if (IsRunning) {
            moveTime += Time.deltaTime;
            currentPosition = moveTime * Speed;
            MoveObjects();
            float texOffset = moveTime / texOffsetFactor;
            RoadMeshRenderer.material.mainTextureOffset = new Vector2(0, -texOffset % 1);
            if (currentPosition >= Length) {
                IsRunning = false;
                EventManager.TriggerEvent(EventNames.RoadFinished, this);
            }
        }
    }
    public float Width;
    public float Length = 40;

    public void AssignRoadObjects(List<RoadObjectBase> objects) {
        roadObjects = objects;
        MoveObjects();
    }
    void MoveObjects() {
        foreach (RoadObjectBase roadObject in roadObjects) {
            if (roadObject != null) {
                float newPos = roadObject.RoadPosition - currentPosition;
                roadObject.transform.position = new Vector3(
                    roadObject.transform.position.x,
                    roadObject.transform.position.y,
                    GetWorldPosition(newPos)
                );
            }
        }
        CleanupObjects();
    }
    float GetWorldPosition(float roadPosition) {
        return ZeroPointInWorld + roadPosition;
    }
    void CleanupObjects() {
        for (int i = 0; i < roadObjects.Count; i++) {
            if (roadObjects[i] == null) {
                roadObjects.RemoveAt(i);
                i--;
            }
        }
    }
}
