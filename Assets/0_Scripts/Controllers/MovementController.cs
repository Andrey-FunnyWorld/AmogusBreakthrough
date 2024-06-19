using UnityEngine;

public abstract class MovementController : MonoBehaviour {
    [HideInInspector]
    public bool AllowMove = true;
    public MainGuy MainGuy;
    public Road Road;

    public float MoveSpeed = 6;
    float roadHalfWidth;
    bool firstMove = false;
    void Start() {
        roadHalfWidth = Road.Width / 2;
    }
    void Update() {
        if (AllowMove) {
            Vector2 inputDir = GetInputDirection();
            if (Mathf.Abs(inputDir.x) > 0) {
                if (!firstMove) {
                    firstMove = true;
                    EventManager.TriggerEvent(EventNames.StartMovement, this);
                }
                float delta = Time.deltaTime * MoveSpeed * Mathf.Sign(inputDir.x);
                Vector3 newPos = MainGuy.transform.position + new Vector3(delta, 0, 0);
                if (CanMove(newPos.x, delta < 0))
                    MainGuy.ApplyMovement(newPos);
            }
        }
    }

    bool CanMove(float newX, bool isLeft) {
        if (isLeft) return MainGuy.Team.TeamRange.Start + newX > -roadHalfWidth;
        else return MainGuy.Team.TeamRange.End + newX < roadHalfWidth;
    }

    protected abstract Vector2 GetInputDirection();
}
