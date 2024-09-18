using System.Collections;
using UnityEngine;

public abstract class Attackable : RoadObjectBase {

    public Animator Animator;
    public Renderer Renderer;
    public AudioSource AudioSource;
    public AudioClip DeathSound;
    [SerializeField] private ParticleSystem hitParticle;
    public bool CanBeAttacked = true;
    const float CORPSE_VISIBLE_DURATION = 1;

    bool playDieFx = true;
    public ProgressBarUI HealthBar;
    public float StartHP = 10;

    float hp = 0;
    const float CORPSE_DECAY_DURATION = 0.5f;
    const float CORPSE_TRANSLATE_DISTANCE = 1.6f;

    public float HP {
        get { return hp; }
        set {
            if (hp != value) {
                hp = value;
                HpChanged(hp);
                HealthBar.Value = hp;
                if (hp <= 0) {
                    Destroyed();
                }
            }
        }
    }
    protected virtual void HpChanged(float newValue) { }
    void Start() {
        Init();
    }
    protected virtual void Init() {
        HP = StartHP;
        HealthBar.MaxValue = StartHP;
        HealthBar.Value = StartHP;
    }
    public void DisableBeenAttacked() {
        CanBeAttacked = false;
    }

    public virtual void Destroyed() {
        if (Animator != null)
            Animator.SetTrigger("die");
        CanBeAttacked = false;
        if (DeathSound != null) {
            AudioSource.clip = DeathSound;
            AudioSource.Play();
        }
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

    protected IEnumerator AnimateCorpseDecay() {
        float timer = 0;
        while (timer < CORPSE_DECAY_DURATION) {
            timer += Time.deltaTime;
            float distance = CORPSE_TRANSLATE_DISTANCE * Time.deltaTime / CORPSE_DECAY_DURATION;
            transform.Translate(0, -distance, 0);
            yield return null;
        }
        CorpseRemoved();
    }
    protected virtual void CorpseRemoved() {
        Destroy(gameObject);
    }
    
    public virtual void TurnOffDieFx() =>
        playDieFx = false;

}
