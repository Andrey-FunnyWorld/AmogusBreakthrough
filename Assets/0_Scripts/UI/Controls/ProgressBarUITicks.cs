using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarUITicks : ProgressBarUI {
    public RectTransform Tick;
    public float LegendIndent = 200;
    List<RectTransform> Ticks = new List<RectTransform>();
    float longDimension = 0;
    public void AddTick(float value, DesintegratorLegendItem legend, bool hideTick = false) {
        float indent = (value / MaxValue) * GetLongSide();
        if (!hideTick) {
            RectTransform newTick = Instantiate(Tick, transform);
            newTick.anchoredPosition += new Vector2(Horizontal ? indent : 0, Horizontal ? 0 : indent);
            Ticks.Add(newTick);
            newTick.gameObject.SetActive(true);
        }
        AdjustTickLegend(indent, legend);
    }
    float GetLongSide() {
        if (longDimension == 0) {
            longDimension = Horizontal ? barRectTransform.rect.width : barRectTransform.rect.height;
        }
        return longDimension;
    }
    void AdjustTickLegend(float indent, DesintegratorLegendItem legend) {
        legend.GetComponent<RectTransform>().anchoredPosition = new Vector2(-LegendIndent, indent);
    }
}
