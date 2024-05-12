using UnityEngine;

public class MainGuy : MonoBehaviour {
    public Team Team;

    void Start() {
        Team.CreateTeam();
    }

    public void ApplyMovement(Vector3 newPosition) {
        transform.position = newPosition;
        Team.ApplyMovement(newPosition);
    }
    
    public void StartMove() {
        // start move animation for the guy and team
    }
}
