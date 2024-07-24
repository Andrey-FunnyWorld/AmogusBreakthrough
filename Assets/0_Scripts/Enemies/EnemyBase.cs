using UnityEngine;

public abstract class EnemyBase : Attackable {
    public float MoveSpeed;
    protected abstract void Attack();

    public override void Destroyed() {
        base.Destroyed();
        // play death animation, give coins and destroy object
    }
    public override void IsRunningChanged(bool isRunning) {
        Animator.SetBool("run", isRunning);
    }
}
