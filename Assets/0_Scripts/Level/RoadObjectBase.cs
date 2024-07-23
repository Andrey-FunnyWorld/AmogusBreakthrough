using System.Collections;
using UnityEngine;

public abstract class RoadObjectBase : MonoBehaviour {
    public ProgressBarUI HealthBar;
    public float RoadPosition;
    public float StartHP = 10;

    float hp = 0;
    const float CORPSE_DECAY_DURATION = 0.5f;
    const float CORPSE_TRANSLATE_DISTANCE = 1.6f;

    public float HP {
        get { return hp; }
        set {
            if (hp != value) {
                hp = value;
                HealthBar.Value = hp;
                if (hp <= 0) {
                    Destroyed();
                }
            }
        }
    }

    void Start() {
        HP = StartHP;
        HealthBar.MaxValue = StartHP;
        HealthBar.Value = StartHP;
    }
    
    public abstract void Destroyed();

    void Update() {
        // only for debug
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            HP -= 5;
        }
    }
    public virtual void IsRunningChanged(bool isRunning) {}

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
}
