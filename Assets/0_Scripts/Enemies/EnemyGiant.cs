using UnityEngine;

public class EnemyGiant : EnemyBase {
    protected override void Attack() {
        Animator.SetTrigger("attack");
        if (AttackSound != null) {
            AudioSource.clip = AttackSound;
            AudioSource.Play();
        }
        // ВАНШОТ, ёбба!
    }
}
