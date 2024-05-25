using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonScored : MonoBehaviour {
    public TextMeshProUGUI ScoreText;
    public ButtonDisabled ButtonDisabled;
    int score = 0;
    public void DefaultClick() {
        SetScore(--UserProgressController.Instance.ProgressState.Spins);
        EventManager.TriggerEvent(EventNames.FreeSpinChanged, score);
    }
    public void SetScore(int newScore) {
        score = newScore;
        ScoreText.text = score.ToString();
        //ButtonDisabled.Enable = score > 0;
    }
    void SpinChanged(object arg) {
        if (score != (int)arg) {
            SetScore((int)arg);
        }
    }
    void Start() {
        EventManager.StartListening(EventNames.FreeSpinChanged, SpinChanged);
    }
    void OnDestroy() {
        EventManager.StopListening(EventNames.FreeSpinChanged, SpinChanged);
    }
}
