using UnityEngine;

public abstract class MovementController : MonoBehaviour {
    [HideInInspector]
    public bool AllowMove = true;
    public MainGuy MainGuy;
    public Road Road;

    public float MoveSpeed = 6;
    float roadHalfWidth, roadScreenWidth;
    bool firstMove = false;
    void Start() {
        roadHalfWidth = Road.Width / 2;
        roadScreenWidth = Road.GetScreenWidth();
    }
    #region New Control System
    protected float noPoint = -10000;
    float prevScreenX;
    void Update() {
        if (AllowMove) {
            Vector2 screenPt = GetScreenPosition();
            if (screenPt.x != noPoint) {
                float screenDelta = prevScreenX != noPoint ? screenPt.x - prevScreenX : 0;
                prevScreenX = screenPt.x;
                if (Mathf.Abs(screenDelta) > 0) {
                    if (!firstMove) {
                        firstMove = true;
                        EventManager.TriggerEvent(EventNames.StartMovement, this);
                    }
                    float delta = screenDelta * Road.Width / roadScreenWidth;
                    Vector3 newPos = MainGuy.transform.position + new Vector3(delta, 0, 0);
                    if (CanMove(newPos.x, screenDelta < 0))
                        MainGuy.ApplyMovement(newPos);
                }
            } else {
                prevScreenX = noPoint;
            }
        }
    }
    protected abstract Vector2 GetScreenPosition();
    #endregion
    // void Update() {
    //     if (AllowMove) {
    //         Vector2 inputDir = GetInputDirection();
    //         if (Mathf.Abs(inputDir.x) > 0) {
    //             if (!firstMove) {
    //                 firstMove = true;
    //                 EventManager.TriggerEvent(EventNames.StartMovement, this);
    //             }
    //             float delta = Time.deltaTime * MoveSpeed * Mathf.Sign(inputDir.x);
    //             Vector3 newPos = MainGuy.transform.position + new Vector3(delta, 0, 0);
    //             if (CanMove(newPos.x, delta < 0))
    //                 MainGuy.ApplyMovement(newPos);
    //         }
    //     }
    // }

    bool CanMove(float newX, bool isLeft) {
        if (isLeft) return MainGuy.Team.TeamRange.Start + newX > -roadHalfWidth;
        else return MainGuy.Team.TeamRange.End + newX < roadHalfWidth;
    }

    //protected abstract Vector2 GetInputDirection();
}
