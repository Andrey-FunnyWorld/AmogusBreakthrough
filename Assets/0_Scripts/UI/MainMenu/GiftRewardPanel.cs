using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GiftRewardPanel : MonoBehaviour {
    public Image GiftImage;
    public TextMeshProUGUI Caption;
    public Sprite PerkSprite, DiamondSprite, MoneySprite, UpgradeSprite;
    public Button UpgradeShopButton, PerkShopButton;
    public ExpandButton ExpandButton;
    public UpgradeShop UpgradeShop;
    public PerkShopList PerkShopList;
    public int MoneyReward, DiamondReward;
    Dictionary<GiftType, Sprite> sprites;
    Dictionary<GiftType, string> localizedCaptions;
    GiftType giftType;
    void Init() {
        if (sprites == null) {
            sprites = new Dictionary<GiftType, Sprite>() {
                { GiftType.Perk, PerkSprite },
                { GiftType.Diamond, DiamondSprite },
                { GiftType.Money, MoneySprite },
                { GiftType.Upgrade, UpgradeSprite }
            };
            localizedCaptions = new Dictionary<GiftType, string>() {
                { GiftType.Perk, MyLocalization.Instance.GetLocalizedText(LocalizationKeys.GiftPerk) },
                { GiftType.Diamond, string.Format(MyLocalization.Instance.GetLocalizedText(LocalizationKeys.GiftDiamond), DiamondReward) },
                { GiftType.Money, string.Format(MyLocalization.Instance.GetLocalizedText(LocalizationKeys.GiftMoney), MoneyReward) },
                { GiftType.Upgrade, MyLocalization.Instance.GetLocalizedText(LocalizationKeys.GiftUpgrade) },
            };
        }
    }
    public void Show(GiftType giftType) {
        this.giftType = giftType;
        gameObject.SetActive(true);
        Init();
        ApplyGiftData(giftType);
    }
    void ApplyGiftData(GiftType giftType) {
        Color color = (giftType == GiftType.Diamond || giftType == GiftType.Money) ? Color.white : Color.red;
        GiftImage.sprite = sprites[giftType];
        GiftImage.color = color;
        Caption.text = localizedCaptions[giftType];
    }
    public void ExecuteGift() {
        switch (giftType) {
            case GiftType.Upgrade: ShowUpgradeShop(); break;
            case GiftType.Perk: ShowPerkShop(); break;
            case GiftType.Money: AddMoney(); break;
            case GiftType.Diamond: AddDiamond(); break;
        }
        gameObject.SetActive(false);
    }
    void ShowUpgradeShop() {
        UpgradeShopButton.onClick.Invoke();
        ExpandButton.ToggleButton();
        UpgradeShop.SetFreeMode(true);
    }
    void ShowPerkShop() {
        PerkShopButton.onClick.Invoke();
        ExpandButton.ToggleButton();
        PerkShopList.SetFreeMode(true);
    }
    void AddMoney() {
        UserProgressController.Instance.ProgressState.Money += MoneyReward;
        HtmlBridge.Instance.SaveProgress();
    }
    void AddDiamond() {
        UserProgressController.Instance.ProgressState.Spins += DiamondReward;
        HtmlBridge.Instance.SaveProgress();
    }
}
