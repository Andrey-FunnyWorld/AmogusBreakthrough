using UnityEngine;

public abstract class Attackable : RoadObjectBase {
    public bool CanBeAttacked = true;

    [SerializeField] private ParticleSystem hitParticle;

    public void DisableBeenAttacked() {
        CanBeAttacked = false;
    }

    public void VisualiseTakeDamage(bool show) {
        if (hitParticle == null || hitParticle.gameObject.activeSelf == show) return;
        
        hitParticle.gameObject.SetActive(show);
    }
}
