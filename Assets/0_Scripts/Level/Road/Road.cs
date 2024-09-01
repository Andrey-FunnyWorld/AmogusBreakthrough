using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Road : MonoBehaviour {
    public float StartSpeed = 2;
    public MeshRenderer RoadMeshRenderer;
    public RunningTexture[] RunningTextures;
    public RoadObjectsGenerator ObjectsGenerator;
    public RoadDecorationGenerator RoadDecorationGenerator;
    [NonSerialized]
    public RoadDataViewModel ViewModel;
    [HideInInspector]
    public float ZeroPointInWorld;

    public float Width;
    public float Length = 40;

    public float tracksCount = 10; //todo
    public Transform BeaconLeft, BeaconRight;
    float roadScreenWidth = 0;

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
        InitTracks();
    }

    void Update() {
        if (IsRunning) {
            moveTime += Time.deltaTime;
            currentPosition = moveTime * Speed;
            NotifyDecorationGenerator();
            NotifyObjectGenerator();
            MoveObjects();
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

    public void StartAction() {
        NotifyObjectGenerator(true);
    }
    // public void AssignRoadObjects(List<RoadObjectBase> objects) {
    //     roadObjects.AddRange(objects);
    //     MoveObjects();
    // }

    public void ApplySlowerMoveSpeedPerk() {
        Speed *= speedReduceFactor;
    }

    public void HandlePerk(PerkType perk) {
        AttackHandler.ApplyPerk(perk);
    }

    public void AbilityOnePunchUsed() {
        AttackHandler.HandleOnePunchAbility(roadObjects);
    }

    [ContextMenu("MakeWeaponBoxesTransparent")] //todo remove later
    public void HandleTransparencyWeaponBoxPerk() {
        foreach (var roadObject in roadObjects) {
            if (roadObject is Weapon weapon) {
                weapon.MakeTransparent();
            }
        }
    }

    void NotifyDecorationGenerator() {
        RoadObjectBase roadObject = RoadDecorationGenerator.PositionChanged(currentPosition);
        if (roadObject != null)
            roadObjects.Add(roadObject);
    }
    void NotifyObjectGenerator(bool move = false) {
        List<RoadObjectBase> objects = ObjectsGenerator.GetObjects(ViewModel, Length, Width, tracksCoords, currentPosition);
        if (objects.Count > 0) {
            roadObjects.AddRange(objects);
        }
        if (move) MoveObjects();
    }
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
            if (roadObject is RoadObstacle obstacle) {
                obstacle.DamageTeam(AttackHandler.MainGuy.Team);
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
    public float GetScreenWidth() {
        if (roadScreenWidth == 0) {
            Camera cam = Camera.main;
            roadScreenWidth = cam.WorldToScreenPoint(BeaconRight.position).x - cam.WorldToScreenPoint(BeaconLeft.position).x;
        }
        return roadScreenWidth;
    }
}
