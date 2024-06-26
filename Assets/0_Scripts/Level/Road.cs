using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour {
    public float StartSpeed = 2;
    public MeshRenderer RoadMeshRenderer;
    [HideInInspector]
    public float ZeroPointInWorld;

    public float Width;
    public float Length = 40;

    public float tracksCount = 10; //todo

    [SerializeField] private AttackController AttackHandler;

    float texOffsetFactor = 0;
    bool isRunning = false;
    float moveTime = 0;
    float currentPosition = 0;
    public List<RoadObjectBase> roadObjects;
    List<float> tracksCoords;
    float speed = 0;

    public bool MovementStarted { get; set; }

    public bool IsRunning { get { return isRunning; }
        set {
            if (isRunning != value) {
                isRunning = value;
                // toggle environment activity if necessary
            }
        }
    }

    public float Speed {
        get { return speed; }
        set {
            if (speed != value) {
                speed = value;
                texOffsetFactor = 10 / ((RoadMeshRenderer.material.mainTextureScale.y / transform.localScale.z) * Speed);
            }
        }
    }

    void Start() {
        Speed = StartSpeed;
    }

    void Update() {
        if (IsRunning) {
            moveTime += Time.deltaTime;
            currentPosition = moveTime * Speed;
            MoveObjects();
            MoveRoadTexture();
            HandleFinishReached();
        }
    }

    public List<float> InitTracks() {
        tracksCoords = new List<float>();

        float roadWidth = Width;
        float trackWidth = roadWidth / tracksCount;
        float nextTrackX = -roadWidth / 2 + roadWidth / (tracksCount * 2);
        tracksCoords.Add(nextTrackX);

        for (int i = 1; i < tracksCount; i++) {
            nextTrackX += trackWidth;
            tracksCoords.Add(nextTrackX);
        }

        return tracksCoords;
    }

    public void PrepareAttackController() {
        AttackHandler.Prepare();
    }

    public void AssignRoadObjects(List<RoadObjectBase> objects) {
        roadObjects = objects;
        MoveObjects();
    }

    void MoveRoadTexture() {
        float texOffset = moveTime / texOffsetFactor;
        RoadMeshRenderer.material.mainTextureOffset = new Vector2(0, -texOffset % 1);
    }

    void HandleFinishReached() {
        if (currentPosition >= Length) {
            IsRunning = false;
            EventManager.TriggerEvent(EventNames.RoadFinished, this);
        }
    }

    void MoveObjects() {
        foreach (RoadObjectBase roadObject in roadObjects) {
            if (roadObject != null) {
                float newPos = roadObject.RoadPosition - currentPosition;
                HandleObjectPosition(roadObject, newPos);
                AttackHandler.HandleAttackObject(roadObject);
                HandleObjectReachedPlayer(roadObject);
            }
        }
        CleanupObjects();
    }

    void HandleObjectPosition(RoadObjectBase roadObject, float newPos) {
        roadObject.transform.position = new Vector3(
            roadObject.transform.position.x,
            roadObject.transform.position.y,
            GetWorldPosition(newPos)
        );
    }

    float GetWorldPosition(float roadPosition) {
        return ZeroPointInWorld + roadPosition;
    }

    void HandleObjectReachedPlayer(RoadObjectBase roadObject) {
        if (AttackHandler.IntersectsTeam(roadObject)) {
            if (roadObject is Weapon weapon) {
                if (!weapon.CanBeAttacked) {
                    weapon.OnPickedUp();
                }
            }

            //else if is EnemyBase...
        }
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
