using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour {
    public string ru, en;
    TMP_Text text;
    void Start() {
        text = GetComponent<TMP_Text>();
        text.text = MyLocalization.Instance.CurrentLanguage == "ru" ? ru : en;
        //EventManager.StartListening(EventNames.LocalizationReady, LocalizationReady);
    }
    void LocalizationReady(object arg) {
        text.text = MyLocalization.Instance.CurrentLanguage == "ru" ? ru : en;
    }
    void OnDestroy() {
        //EventManager.StopListening(EventNames.LocalizationReady, LocalizationReady);
    }
}
