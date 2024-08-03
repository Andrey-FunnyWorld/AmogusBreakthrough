using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour {
    public float StartSpeed = 2;
    public MeshRenderer RoadMeshRenderer;
    public RunningTexture[] RunningTextures;
    public RoadDecorationGenerator RoadDecorationGenerator;
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
    float speedReduceFactor = 0.75f;
    bool finished = false;

    public bool MovementStarted { get; set; }

    public bool IsRunning { get { return isRunning; }
        set {
            if (isRunning != value) {
                isRunning = value;
                // toggle environment activity if necessary
                foreach (RoadObjectBase roadObject in roadObjects)
                    roadObject.IsRunningChanged(isRunning);
                foreach (RunningTexture tex in RunningTextures)
                    tex.IsRunning = isRunning;
            }
        }
    }

    public float Speed {
        get { return speed; }
        set {
            if (speed != value) {
                speed = value;
                foreach (RunningTexture tex in RunningTextures)
                    tex.SetSpeed(speed);
            }
        }
    }

    void Start() {
        Speed = StartSpeed;
        roadObjects.AddRange(RoadDecorationGenerator.GenerateStartDecoration());
    }

    void Update() {
        if (IsRunning) {
            moveTime += Time.deltaTime;
            currentPosition = moveTime * Speed;
            NotifyDecorationGenerator();
            MoveObjects();
            //MoveRoadTexture();
            HandleFinishReached();
        }
    }

    void NotifyDecorationGenerator() {
        RoadObjectBase roadObject = RoadDecorationGenerator.PositionChanged(currentPosition);
        if (roadObject != null)
            roadObjects.Add(roadObject);
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
        roadObjects.AddRange(objects);
        MoveObjects();
    }

    public void ApplySlowerMoveSpeedPerk() {
        Speed *= speedReduceFactor;
    }

    public void HandlePerk(PerkType perk) {
        AttackHandler.ApplyPerk(perk);
    }

    // void MoveRoadTexture() {
    //     float texOffset = moveTime / texOffsetFactor;
    //     RoadMeshRenderer.material.mainTextureOffset = new Vector2(0, -texOffset % 1);
    // }

    void HandleFinishReached() {
        if (!finished && currentPosition >= Length) {
            IsRunning = false;
            finished = true;
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
        // remove destroyed objects
        for (int i = 0; i < roadObjects.Count; i++) {
            if (roadObjects[i] == null) {
                roadObjects.RemoveAt(i);
                i--;
            }
        }
        // remove objects you passed by
        for (int i = 0; i < roadObjects.Count; i++) {
            if (roadObjects[i].RoadPosition < currentPosition - 10) {
                Destroy(roadObjects[i].gameObject);
                roadObjects.RemoveAt(i);
            }
        }
    }
}
