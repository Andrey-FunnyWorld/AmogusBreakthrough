using UnityEngine;

public abstract class EnemyBase : RoadObjectBase {
    public float MoveSpeed;
    [SerializeField] private ParticleSystem hitParticle;
    protected abstract void Attack();
    public override void Destroyed() {
        // play death animation, give coins and destroy object
        Destroy(gameObject);
    }

    public void VisualiseTakingDamage(bool show) {
        if (hitParticle == null || hitParticle.gameObject.activeSelf == show) return;
        
        hitParticle.gameObject.SetActive(show);
    }
}
