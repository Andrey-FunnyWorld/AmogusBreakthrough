using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAutoSizer : MonoBehaviour {
    int lastScreenWidth = 0;
    int lastScreenHeight = 0;
    void Update() {
        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height) {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            Adjust(lastScreenWidth, lastScreenHeight);
        }
    }
    protected abstract void Adjust(int width, int height);
}
