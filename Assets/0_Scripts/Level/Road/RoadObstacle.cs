using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObstacle : RoadObjectBase {
    public AudioSource DamageSound;
    public float Damage = 40;
    bool canDamage = true;
    public void DamageTeam(Team team) {
        if (canDamage) {
            canDamage = false;
            team.TeamHealth.TakeDamage(Damage);
            DamageSound.Play();
        }
    }
}
