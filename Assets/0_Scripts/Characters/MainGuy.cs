using UnityEngine;

public class MainGuy : MonoBehaviour {
    public Team Team;
    public float Damage = 0.2f;

    public void ApplyMovement(Vector3 newPosition) {
        transform.position = newPosition;
        Team.ApplyMovement(newPosition);
    }
    
    public void StartMove() {
        // start move animation for the guy and team
    }
    public void ApplyProgress(ProgressState progress) {
        Team.CreateTeam(progress);
    }
}
