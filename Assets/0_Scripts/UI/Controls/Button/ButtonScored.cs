using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(ButtonDisabled))]
public class ButtonScored : MonoBehaviour {
    public TextMeshProUGUI ScoreText;
    public ButtonDisabled ButtonDisabled;
    int score = -1;
    public int Score {
        get { return score; }
        set {
            if (value != score) {
                score = value;
                ScoreText.text = score.ToString();
                ButtonDisabled.Enable = score > 0;
            }
        }
    }
    public void DefaultClick() {
        if (Score > 0) {
            Score = --UserProgressController.Instance.ProgressState.Spins;
            EventManager.TriggerEvent(EventNames.FreeSpinChanged, score);
        }
    }
    void SpinChanged(object arg) {
        if (score != (int)arg) {
            Score = (int)arg;
        }
    }
    void Start() {
        EventManager.StartListening(EventNames.FreeSpinChanged, SpinChanged);
    }
    void OnDestroy() {
        EventManager.StopListening(EventNames.FreeSpinChanged, SpinChanged);
    }
}
