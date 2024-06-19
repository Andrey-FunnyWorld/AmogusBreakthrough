using UnityEngine;

public abstract class RoadObjectBase : MonoBehaviour {
    public ProgressBarUI HealthBar;
    public float RoadPosition;
    public float StartHP = 10;

    float hp = 0;

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
}
