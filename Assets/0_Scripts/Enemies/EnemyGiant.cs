using UnityEngine;

public class EnemyGiant : EnemyBase {

    private Team _team;
    
    public override void Attack(Team team) {
        if (HP <= 0)
            return;
        Attack();
    }

    protected override void Attack() {
        Animator.SetTrigger("attack");
        if (AttackSound != null) {
            AudioSource.clip = AttackSound;
            AudioSource.Play();
        }
        // ВАНШОТ, ёбба!
    }

    public void OnAttackFinish() {
        _team?.TeamHealth.TakeDamage(1000f);
        _team = null;
    }
}
