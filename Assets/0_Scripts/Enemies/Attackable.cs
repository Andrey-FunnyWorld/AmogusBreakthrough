using UnityEngine;

public abstract class Attackable : RoadObjectBase {

    public Animator Animator;
    public Renderer Renderer;
    [SerializeField] private ParticleSystem hitParticle;
    public bool CanBeAttacked = true;
    const float CORPSE_VISIBLE_DURATION = 1;

    bool playDieFx = true;

    public void DisableBeenAttacked() =>
        CanBeAttacked = false;

    public override void Destroyed() {
        if (Animator != null)
            Animator.SetTrigger("die");
        CanBeAttacked = false;
        if (playDieFx) {
            StartCoroutine(Utils.WaitAndDo(CORPSE_VISIBLE_DURATION, () => {
                StartCoroutine(AnimateCorpseDecay());
            }));
        }
    }

    public void VisualiseTakeDamage(bool show) {
        if (hitParticle == null || hitParticle.gameObject.activeSelf == show) return;
        hitParticle.gameObject.SetActive(show);
    }

    protected virtual void TurnOffDieFx() =>
        playDieFx = false;

}
