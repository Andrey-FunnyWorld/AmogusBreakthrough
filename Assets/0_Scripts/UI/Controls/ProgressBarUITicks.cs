using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarUITicks : ProgressBarUI {
    public RectTransform Tick;
    public float LegendIndent = 200;
    List<RectTransform> Ticks = new List<RectTransform>();
    List<float> tickValues = new List<float>();
    List<DesintegratorLegendItem> legendItems = new List<DesintegratorLegendItem>();
    float longDimension = 0;
    public void AddTick(float value, DesintegratorLegendItem legend, bool hideTick = false) {
        if (value == 1) value = 3;
        tickValues.Add(value);
        float indent = (value / MaxValue) * GetLongSide();
        if (!hideTick) {
            RectTransform newTick = Instantiate(Tick, transform);
            newTick.anchoredPosition += new Vector2(Horizontal ? indent : 0, Horizontal ? 0 : indent);
            Ticks.Add(newTick);
            newTick.gameObject.SetActive(true);
        }
        legendItems.Add(legend);
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
    public void AdjustTicks() {
        longDimension = 0;
        for (int i = 0; i < Ticks.Count; i++) {
            RectTransform tick = Ticks[i];
            float value = tickValues[i];
            float indent = GetLongSide() * value / MaxValue;
            tick.anchoredPosition = Tick.anchoredPosition + new Vector2(Horizontal ? indent : 0, Horizontal ? 0 : indent);
            AdjustTickLegend(indent, legendItems[i]);
        }
    }
}
