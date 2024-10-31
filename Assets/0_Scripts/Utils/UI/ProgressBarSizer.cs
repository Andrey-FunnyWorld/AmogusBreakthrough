using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProgressBarUITicks))]
public class ProgressBarSizer : BaseAutoSizer {
    ProgressBarUITicks bar;
    void Start() {
        bar = GetComponent<ProgressBarUITicks>();
    }
    protected override void Adjust(int width, int height) {
        float dimensionValue = (bar.Horizontal ? bar.barRectTransform.rect.width : bar.barRectTransform.rect.height) * (bar.Value / bar.MaxValue);
        bar.progressImage.SetSizeWithCurrentAnchors(bar.Horizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical, dimensionValue);
        // ticks
        bar.AdjustTicks();
    }
}
