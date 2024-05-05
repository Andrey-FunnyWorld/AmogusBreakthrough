using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGuy : MonoBehaviour {
    public Team Team;
    public void ApplyMovement(Vector3 newPosition) {
        transform.position = newPosition;
        Team.ApplyMovement(newPosition);
    }
    public void StartMove() {
        // start move animation for the guy and team
    }
    void Start() {
        Team.CreateTeam();
    }
}
