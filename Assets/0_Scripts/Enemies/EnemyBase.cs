using UnityEngine;

public abstract class EnemyBase : Attackable {
    public float MoveSpeed;
    // [SerializeField] private ParticleSystem hitParticle;
    protected abstract void Attack();

    public override void Destroyed() {
        // play death animation, give coins and destroy object
        Destroy(gameObject);
    }

    // public void VisualiseTakeDamage(bool show) {
    //     if (hitParticle == null || hitParticle.gameObject.activeSelf == show) return;
        
    //     hitParticle.gameObject.SetActive(show);
    // }
}
