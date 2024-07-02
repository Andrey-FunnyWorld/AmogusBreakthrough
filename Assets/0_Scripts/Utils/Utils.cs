using System.Collections;
using System.Collections.Generic;
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
}
