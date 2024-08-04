using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    public MainGuy MainGuy;
    public ShopList ShopBackpacks, ShopHats;
    public PerkShopList PerkShopList;
    public UpgradeShop UpgradeShop;
    public Transform MainMenu;
    public ButtonDisabled[] DisableWhenSpinning;
    public ProgressText3D HatText, BackpackText;
    public ScoreTextMenu ScoreText;
    public SettingsPanel SettingsPanel;
    public SkipAdButton SkipAdButton;
    public Wheel Wheel;
    //List<ButtonDisabled> buttonsToSkip = new List<ButtonDisabled>();
    void Awake() {
        SubscriveEvents();
    }
    void Start() {
        if (UserProgressController.ProgressLoaded)
            ApplyProgressAll();
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
    int CalcSkinProgress(SkinType skinType, ProgressState progress) {
        SkinItems skinItems = skinType == SkinType.Hat ? ShopHats.Items  : ShopBackpacks.Items;
        int maxItems = skinType == SkinType.Hat ? progress.PurchasedHats.Length : progress.PurchasedBackpacks.Length;
        return (int)Math.Floor(100 * (float)maxItems / (float)skinItems.Items.Length);
    }
    void ApplyProgress(ProgressState progress) {
        MainGuy.ApplyProgress(progress);
        UpdateProgressTexts(progress);
        ScoreText.SetScoreSilent(progress.Money);
        Wheel.ApplyProgress(progress);
    }
    void ChooseQualityLevel(PlatformType platform) {
        int currentLevel = QualitySettings.GetQualityLevel();
        int desiredIndex;
        switch (platform) {
            case PlatformType.Android: desiredIndex = 0; break;
            case PlatformType.IOS: desiredIndex = 1; break;
            default: desiredIndex = 2; break;
        }
        if (desiredIndex != currentLevel) {
            QualitySettings.SetQualityLevel(desiredIndex);
        }
    }
    void UpdateProgressTexts(ProgressState progress) {
        HatText.SetProgress(CalcSkinProgress(SkinType.Hat, progress));
        BackpackText.SetProgress(CalcSkinProgress(SkinType.Backpack, progress));
    }
    void ApplyProgressLight(ProgressState progress) {
        ScoreText.Score = progress.Money;
    }
    void ShopItemPurchased(object arg) {
        ListItem listItem = (ListItem)arg;
        UserProgressController.Instance.ProgressState.AddPurchased(listItem.ShopType, listItem.Model.SkinName);
        UserProgressController.Instance.ProgressState.Money -= listItem.Model.Price;
        UpdateProgressTexts(UserProgressController.Instance.ProgressState);
        ApplyProgressLight(UserProgressController.Instance.ProgressState);
        UserProgressController.Instance.SaveProgress();
        HtmlBridge.Instance.ReportMetric(MetricNames.SkinPurchased);
    }
    void PerkItemPurchased(object arg) {
        PerkModel model = (PerkModel)arg;
        UserProgressController.Instance.ProgressState.AddPurchased(model.PerkType);
        UserProgressController.Instance.ProgressState.Money -= model.Price;
        ApplyProgressLight(UserProgressController.Instance.ProgressState);
        UserProgressController.Instance.SaveProgress();
        HtmlBridge.Instance.ReportMetric(MetricNames.PerkPurchased);
    }
    void UpgradeItemPurchased(object arg) {
        UpgradeItem upgradeItem = (UpgradeItem)arg;
        UserProgressController.Instance.ProgressState.Money -= upgradeItem.Price;
        UserProgressController.Instance.ProgressState.AddUpgrade(upgradeItem.UpgradeType, upgradeItem.CurrentLevel);
        ApplyProgressLight(UserProgressController.Instance.ProgressState);
        UserProgressController.Instance.SaveProgress();
        HtmlBridge.Instance.ReportMetric(MetricNames.UpgradePurchased);
    }
    void SkipAdPurchased(object arg) {
        ApplyProgressLight(UserProgressController.Instance.ProgressState);
    }
    void ApplyProgressAll() {
        ApplyProgress(UserProgressController.Instance.ProgressState);
        SettingsPanel.ApplyProgress(UserProgressController.Instance.PlayerSettings);
        ShopBackpacks.GenerateItems(UserProgressController.Instance.ProgressState);
        ShopHats.GenerateItems(UserProgressController.Instance.ProgressState);
        PerkShopList.GenerateItems(UserProgressController.Instance.ProgressState);
        UpgradeShop.ApplyProgress(UserProgressController.Instance.ProgressState);
        SkipAdButton.ApplyProgress(UserProgressController.Instance.ProgressState.SkipAdRounds);
        EventManager.TriggerEvent(EventNames.LevelLoaded, this);
    }
    void StartDataLoaded(object arg) {
        ApplyProgressAll();
        ChooseQualityLevel(HtmlBridge.PlatformType);
    }
    public void ShowShopAction(bool show) {
        MainMenu.gameObject.SetActive(!show);
    }
    void NotEnoughMoney(object arg) {
        ScoreText.HighlightError();
    }
    void WheelSpinStart(object arg) {
        // foreach (ButtonDisabled btn in DisableWhenSpinning) {
        //     if (!btn.Enable) buttonsToSkip.Add(btn);
        //     else btn.Enable = false;
        // }
    }
    void WheelSpinResult(object arg) {
        // foreach (ButtonDisabled btn in DisableWhenSpinning) {
        //     if (!buttonsToSkip.Contains(btn))
        //         btn.Enable = true;
        // }
    }
    void SkinItemEquip(object arg) {
        SkinItemEquipArgs args = (SkinItemEquipArgs)arg;
        UserProgressController.Instance.ProgressState.Equipp(args.ShopType, args.ItemModel.SkinName);
        UserProgressController.Instance.SaveProgress();
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StartListening(EventNames.WheelSpinStart, WheelSpinStart);
        EventManager.StartListening(EventNames.WheelSpinResult, WheelSpinResult);
        EventManager.StartListening(EventNames.ShopItemPurchased, ShopItemPurchased);
        EventManager.StartListening(EventNames.PerkItemPurchased, PerkItemPurchased);
        EventManager.StartListening(EventNames.UpgradeItemPurchased, UpgradeItemPurchased);
        EventManager.StartListening(EventNames.NotEnoughMoney, NotEnoughMoney);
        EventManager.StartListening(EventNames.SkipAdPurchased, SkipAdPurchased);
        EventManager.StartListening(EventNames.SkinItemEquip, SkinItemEquip);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StopListening(EventNames.WheelSpinStart, WheelSpinStart);
        EventManager.StopListening(EventNames.WheelSpinResult, WheelSpinResult);
        EventManager.StopListening(EventNames.ShopItemPurchased, ShopItemPurchased);
        EventManager.StopListening(EventNames.PerkItemPurchased, PerkItemPurchased);
        EventManager.StopListening(EventNames.UpgradeItemPurchased, UpgradeItemPurchased);
        EventManager.StopListening(EventNames.NotEnoughMoney, NotEnoughMoney);
        EventManager.StopListening(EventNames.SkipAdPurchased, SkipAdPurchased);
        EventManager.StopListening(EventNames.SkinItemEquip, SkinItemEquip);
    }
    public void HideShops() {
        ShopBackpacks.gameObject.SetActive(false);
        ShopHats.gameObject.SetActive(false);
        PerkShopList.gameObject.SetActive(false);
        UpgradeShop.gameObject.SetActive(false);
    }
}
