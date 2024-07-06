using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdSpinButton : MonoBehaviour {
    public int NextSpinDelaySec = 15;
    public TextMeshProUGUI Text;
    public ButtonDisabled ButtonDisabled;
    [NonSerialized]
    public DateTime DateAvailable;
    public void ApplyFinishDate(DateTime date) {
        Text.text = GetTimeText(date);
        DateAvailable = date;
        ButtonDisabled.Enable = DateAvailable < DateTime.Now;
    }
    public void Spin() {
        DateAvailable = DateTime.Now.AddSeconds(NextSpinDelaySec);
        UserProgressController.Instance.ProgressState.AdSpinWhenAvailableString = DateAvailable.ToString();
        UserProgressController.Instance.SaveProgress();
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
        ButtonDisabled.Enable = DateAvailable < DateTime.Now;
    }
}
