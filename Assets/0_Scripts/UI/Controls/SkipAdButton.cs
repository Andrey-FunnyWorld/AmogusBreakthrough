using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipAdButton : MonoBehaviour {
    public MyDialog BuySkipAdDialog;
    public TMPro.TextMeshProUGUI PriceText, RoundText;
    public int SkipAdCount = 2;
    public int Price;
    public Transform BuyPart, RoundPart;
    public void ShowDialog() {
        string question = BuySkipAdDialog.Question.text.Replace("XXX", SkipAdCount.ToString());
        string yesText = string.Format("{0} {1} <sprite name=\"coin\">", MyLocalization.Instance.GetLocalizedText(LocalizationKeys.Yes), Price.ToString());
        BuySkipAdDialog.Show(BuySkipAdDialog.Caption.text, question, yesText, () => {
            BuyAdSkip();
        });
    }
    void BuyAdSkip() {
        if (UserProgressController.Instance.ProgressState.Money >= Price) {
            UserProgressController.Instance.ProgressState.SkipAdRounds += SkipAdCount;
            UserProgressController.Instance.ProgressState.Money -= Price;
            UserProgressController.Instance.SaveProgress();
            EventManager.TriggerEvent(EventNames.SkipAdPurchased, this);
            ApplyProgress(UserProgressController.Instance.ProgressState.SkipAdRounds);
        } else EventManager.TriggerEvent(EventNames.NotEnoughMoney, this);
    }
    string priceFormat = "{0} <sprite name=\"coin\">";
    void Start() {
        PriceText.text = string.Format(priceFormat, Price.ToString());
    }
    public void ApplyProgress(int roundCount) {
        BuyPart.gameObject.SetActive(roundCount == 0);
        RoundPart.gameObject.SetActive(roundCount != 0);
        RoundText.text = roundCount.ToString();
    }
}
