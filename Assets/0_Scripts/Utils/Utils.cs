using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public static class Utils {
    public static IEnumerator WaitAndDo(float time, UnityAction callback) {
        float timer = 0;
        while (timer < time) {
            timer += Time.deltaTime;
            yield return null;
        }
        callback.Invoke();
    }
    public static float EaseOutCubic(float x) {
        return 1 - Mathf.Pow(x, 3);
    }
    public static float EaseInCubic(float x) {
        return Mathf.Pow(x, 3);
    }
    public static float EaseInSquare(float x) {
        return Mathf.Pow(x, 2);
    }
    public static float Gravity = 9.8f;
    public static IEnumerator ChainActions(List<ChainedAction> actions) {
        float timer = 0;
        int index = 0;
        float stepTime = actions[0].DeltaTime;
        while (index < actions.Count) {
            timer += Time.deltaTime;
            if (timer > stepTime) {
                actions[index].Callback();
                index++;
                if (index < actions.Count)
                    stepTime += actions[index].DeltaTime;
            }
            yield return null;
        }
    }
    public static IEnumerator AnimateScale(float time, Transform tr, float extent, bool scaleUp) {
        Vector3 pivotScale = tr.localScale;
        Vector3 originalScale = tr.localScale;
        bool forward = true;
        float timer = 0;
        while (timer < time / 2) {
            timer += Time.deltaTime;
            float diff = (scaleUp ? 1 : -1) * extent * timer / time;
            tr.localScale = pivotScale + new Vector3(diff, diff, diff);
            if (forward && timer > time / 2) {
                forward = false;
                timer = 0;
                scaleUp = !scaleUp;
                pivotScale = tr.localScale;
            }
            yield return null;
        }
        tr.localScale = originalScale;
    }
    public static IEnumerator AnimateScaleLoop(float time, Transform tr, float extent, bool scaleUp) {
        Vector3 pivotScale = tr.localScale;
        bool forward = true;
        float timer = 0;
        while (true) {
            timer += Time.deltaTime;
            float diff = (scaleUp ? 1 : -1) * extent * timer / time;
            tr.localScale = pivotScale + new Vector3(diff, diff, diff);
            if (timer > time) {
                forward = !forward;
                timer = 0;
                scaleUp = !scaleUp;
                pivotScale = tr.localScale;
            }
            yield return null;
        }
    }
}
public class ChainedAction {
    public float DeltaTime;
    public UnityAction Callback;
}