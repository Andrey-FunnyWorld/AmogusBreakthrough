using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoserAssistant : MonoBehaviour {
    public MyDialog UpgradeDialog;
    public MyDialog GiftDialog;
    public Button UpgradeButton;
    public GiftBoxButton GiftBoxButton;
    public ExpandButton ExpandButton;
    public UpgradeShop UpgradeShop;
    public int LostRoundsForFirstRecommendation = 1;
    public int LostRoundsLoop = 3;
    public int RoundsPlayedToGift = 10;
    public static int LostRounds = 0;
    public static int RoundsPlayed = 0;
    static bool GiftRecommended = false;
    public void RecommendUpgrade() {
        if (UserProgressController.Instance.ProgressState.Money >= UpgradeShop.UpgradePrice) {
            UpgradeDialog.Show(() => {
                UpgradeButton.onClick.Invoke();
                ExpandButton.ToggleButton();
            });
        }
    }
    public void CheckToRecommend() {
        if (!GiftRecommended) GiftRecommended = RoundsPlayed == RoundsPlayedToGift;
        if (LostRounds == LostRoundsForFirstRecommendation) {
            RecommendUpgrade();
        } else if ((LostRounds - LostRoundsForFirstRecommendation) % LostRoundsLoop == 0) {
            RecommendUpgrade();
        } else if (GiftRecommended) {
            GiftRecommended = false;
            GiftDialog.Show(() => { GiftBoxButton.DropGift(); });
        }
    }
}
