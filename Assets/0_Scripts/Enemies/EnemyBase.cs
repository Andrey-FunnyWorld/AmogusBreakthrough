using UnityEngine;

public abstract class EnemyBase : Attackable {
    public float Damage;
    public float MoveSpeed;
    public AudioClip AttackSound;
    public abstract void Attack(Team team, float roadSpeed);

    public override void Destroyed() {
        base.Destroyed();
        // play death animation, give coins and destroy object
    }
    public override void IsRunningChanged(bool isRunning) {
        Animator.SetBool("run", isRunning);
    }
}
