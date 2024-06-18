using UnityEngine;

public class MainGuy : TeamMember {
    public Team Team;
    public WeaponType CurrentWeaponType;
    public float DamageMultiplier = 1.5f;

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
