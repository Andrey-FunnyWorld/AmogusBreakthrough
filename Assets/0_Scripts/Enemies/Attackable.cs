using UnityEngine;

public abstract class Attackable : RoadObjectBase {
    public Renderer Renderer;
    public bool CanBeAttacked = true;
    const float CORPSE_VISIBLE_DURATION = 1;

    [SerializeField] private ParticleSystem hitParticle;

    public void DisableBeenAttacked() {
        CanBeAttacked = false;
    }
    public Animator Animator;
    public override void Destroyed() {
        if (Animator != null)
            Animator.SetTrigger("die");
        CanBeAttacked = false;
        StartCoroutine(Utils.WaitAndDo(CORPSE_VISIBLE_DURATION, () => {
            StartCoroutine(AnimateCorpseDecay());
        }));
    }

    public void VisualiseTakeDamage(bool show) {
        if (hitParticle == null || hitParticle.gameObject.activeSelf == show) return;
        hitParticle.gameObject.SetActive(show);
    }

}
