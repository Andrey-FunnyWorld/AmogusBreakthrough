using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAppTracker : MonoBehaviour {
    void OnApplicationFocus(bool focus) {
        if (!HtmlBridge.AdsIsVisible)
            AudioListener.volume = focus ? 1.0f : 0.0f;
    }
}
