using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySimple : EnemyBase {
    protected override void Attack() {
        Animator.SetTrigger("attack");
        if (AttackSound != null) {
            AudioSource.clip = AttackSound;
            AudioSource.Play();
        }
        // apply damage to the team
    }
}
