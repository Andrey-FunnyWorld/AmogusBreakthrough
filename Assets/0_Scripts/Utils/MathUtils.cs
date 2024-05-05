using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils {
    public static float EaseOutCubic(float x) {
        return 1 - Mathf.Pow(1 - x, 3);
    }
}
