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
}
