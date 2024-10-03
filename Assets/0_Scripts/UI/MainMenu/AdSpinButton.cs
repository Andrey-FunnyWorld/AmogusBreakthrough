using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AdSpinButton : MonoBehaviour {
    public int NextSpinDelaySec = 15;
    public TextMeshProUGUI Text;
    public ButtonDisabled ButtonDisabled;
    public bool KeepDisabled = false;
    [NonSerialized]
    public DateTime DateAvailable;
    public Wheel Wheel;
    public float SpinDuration = 7;
    public void ApplyFinishDate(DateTime date) {
        Text.text = GetTimeText(date);
        DateAvailable = date;
        ButtonDisabled.Enable = DateAvailable < DateTime.Now;
    }
    public void Spin() {
        HtmlBridge.Instance.ReportMetric(MetricNames.RewardWheel);
        #if UNITY_WEBGL && !UNITY_EDITOR
        HtmlBridge.Instance.ShowRewarded(() => {
            SpinStarter();
        }, () => {});
        #endif
        #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN
            SpinStarter();
        #endif
    }
    void SpinStarter() {
        DateAvailable = DateTime.Now.AddSeconds(NextSpinDelaySec);
        Text.text = GetTimeText(DateAvailable);
        UserProgressController.Instance.ProgressState.AdSpinWhenAvailableString = DateAvailable.ToString();
        UserProgressController.Instance.SaveProgress();
        Wheel.Spin(SpinDuration);
    }
    string GetTimeText(DateTime date) {
        if (date < DateTime.Now) {
            return MyLocalization.Instance.GetLocalizedText(LocalizationKeys.AdSpinReady);
        } else {
            TimeSpan time = date - DateTime.Now;
            return string.Format("{0}:{1}", time.Minutes, time.Seconds > 9 ? time.Seconds : "0" + time.Seconds);
        }
    }
    public void TimerTick() {
        Text.text = GetTimeText(DateAvailable);
        if (!KeepDisabled)
            ButtonDisabled.Enable = DateAvailable < DateTime.Now;
    }
    public bool IsTimerRunning {
        get { return DateAvailable >= DateTime.Now; }
    }
}
