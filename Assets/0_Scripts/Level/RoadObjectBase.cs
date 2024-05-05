using System.Collections;
using UnityEngine;

public abstract class RoadObjectBase : MonoBehaviour {
    public ProgressBarUI HealthBar;
    public float RoadPosition;
    void Start() {
        HP = StartHP;
        HealthBar.MaxValue = StartHP;
        HealthBar.Value = StartHP;
    }
    public float StartHP = 10;
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
    float hp = 0;
    public abstract void Destroyed();
    void Update() {
        // only for debug
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            HP -= 5;
        }
    }
}
