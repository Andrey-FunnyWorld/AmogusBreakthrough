using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopScaler : MonoBehaviour {
    public bool RunOnStart = true;
    public float Min = 0.7f;
    public float Max = 1.3f;
    public float HalfLoopDuration = 0.5f;
    
    bool isRunning = false;
    Coroutine runCoroutine = null;
    Vector3 originalScale;
    void Start() {
        originalScale = transform.localScale;
        IsRunning = RunOnStart;
    }
    public bool IsRunning {
        get { return isRunning; }
        set {
            if (isRunning != value) {
                isRunning = value;
                if (runCoroutine != null) StopCoroutine(runCoroutine);
                if (isRunning)
                    if (gameObject.activeInHierarchy)
                        runCoroutine = StartCoroutine(Scale());
                else {
                    transform.localScale = originalScale;
                }
            }
        }
    }
    IEnumerator Scale() {
        float timer = 0;
        bool scaleUp = true;
        Vector3 pivotScale = originalScale;
        float scaleLength = Max - Min;
        while (true) {
            timer += Time.deltaTime;
            float diff = (scaleUp ? 1 : -1) * scaleLength * timer / HalfLoopDuration;
            transform.localScale = pivotScale + new Vector3(diff, diff, diff);
            if (timer > HalfLoopDuration) {
                timer = 0;
                scaleUp = !scaleUp;
                pivotScale = transform.localScale;
            }
            yield return null;
        }
    }
    void OnEnable() {
        if (isRunning) runCoroutine = StartCoroutine(Scale());
    }
    void OnDisable() {
        if (runCoroutine != null) StopCoroutine(runCoroutine);
    }
}
