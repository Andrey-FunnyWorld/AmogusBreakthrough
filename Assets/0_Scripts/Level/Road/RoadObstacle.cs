using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObstacle : RoadObjectBase {
    public float Damage = 40;
    bool canDamage = true;
    public void DamageTeam(Team team) {
        if (canDamage) {
            canDamage = false;
            Debug.Log(transform.position);
            team.TeamHealth.TakeDamage(Damage);
        }
    }
}
