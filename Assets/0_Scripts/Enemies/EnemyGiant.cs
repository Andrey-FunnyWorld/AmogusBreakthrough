using UnityEngine;

public class EnemyGiant : EnemyBase {
    protected override void Attack() {
        Animator.SetTrigger("attack");
        // ВАНШОТ, ёбба!
    }
}
