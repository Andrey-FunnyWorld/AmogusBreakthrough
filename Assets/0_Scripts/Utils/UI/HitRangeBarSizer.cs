using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitRangeBar))]
public class HitRangeBarSizer : BaseAutoSizer {
    HitRangeBar bar;
    void Start() {
        bar = GetComponent<HitRangeBar>();
    }
    protected override void Adjust(int width, int height) {
        bar.TargetArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bar.BarArea.rect.width * bar.TargetRatio);
    }
}
