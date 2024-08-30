using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesintegratorLegendItem : MonoBehaviour {
    public RectTransform CompletedIcon;
    bool completed = false;
    public bool Completed {
        get { return completed; }
        set {
            completed = value;
            CompletedIcon.gameObject.SetActive(completed);
        }
    }
}
