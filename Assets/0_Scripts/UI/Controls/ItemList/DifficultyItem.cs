using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DifficultyItem : MonoBehaviour, IPointerDownHandler {
    public Difficulty Difficulty;
    public LevelLoader LevelLoader;
    bool canPress = true;
    public void OnPointerDown(PointerEventData arg) {
        if (canPress) {
            canPress = false;
            LevelLoader.Difficulty = Difficulty;
            string metricName = string.Empty;
            switch (Difficulty) {
                case Difficulty.Noob: metricName = MetricNames.DifficultyNoob; break;
                case Difficulty.Pro: metricName = MetricNames.DifficultyPro; break;
                case Difficulty.Hacker: metricName = MetricNames.DifficultyHacker; break;
            }
            HtmlBridge.Instance.ReportMetric(metricName);
            LevelLoader.LoadScene(LevelLoader.BATTLE_BUILD_INDEX);
        }
    }
}
public enum Difficulty {
    Noob, Pro, Hacker
}