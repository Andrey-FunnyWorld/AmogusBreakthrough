using UnityEngine;

public abstract class EnemyBase : Attackable {
    public float MoveSpeed;
    public Animator Animator;
    // [SerializeField] private ParticleSystem hitParticle;
    protected abstract void Attack();

    public override void Destroyed() {
        base.Destroyed();
        // play death animation, give coins and destroy object
        Animator.SetTrigger("die");
    }
    public override void IsRunningChanged(bool isRunning) {
        Animator.SetBool("run", isRunning);
    }

    // public void VisualiseTakeDamage(bool show) {
    //     if (hitParticle == null || hitParticle.gameObject.activeSelf == show) return;

    //     hitParticle.gameObject.SetActive(show);
    // }
}
