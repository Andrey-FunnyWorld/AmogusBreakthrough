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
    List<ButtonDisabled> buttonsToSkip = new List<ButtonDisabled>();
    void Start() {
        SubscriveEvents();
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
    }
    void PerkItemPurchased(object arg) {
        PerkModel model = (PerkModel)arg;
        UserProgressController.Instance.ProgressState.AddPurchased(model.PerkType);
        UserProgressController.Instance.ProgressState.Money -= model.Price;
        UpdateProgressTexts(UserProgressController.Instance.ProgressState);
        ApplyProgressLight(UserProgressController.Instance.ProgressState);
    }
    void StartDataLoaded(object arg) {
        ApplyProgress(UserProgressController.Instance.ProgressState);
        ShopBackpacks.GenerateItems(UserProgressController.Instance.ProgressState);
        ShopHats.GenerateItems(UserProgressController.Instance.ProgressState);
        PerkShopList.GenerateItems(UserProgressController.Instance.ProgressState);
    }
    public void ShowShopAction(bool show) {
        MainMenu.gameObject.SetActive(!show);
    }
    void NotEnoughMoney(object arg) {
        ScoreText.HighlightError();
    }
    void WheelSpinStart(object arg) {
        foreach (ButtonDisabled btn in DisableWhenSpinning) {
            if (!btn.Enable) buttonsToSkip.Add(btn);
            else btn.Enable = false;
        }
    }
    void WheelSpinResult(object arg) {
        foreach (ButtonDisabled btn in DisableWhenSpinning) {
            if (!buttonsToSkip.Contains(btn))
                btn.Enable = true;
        }
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StartListening(EventNames.WheelSpinStart, WheelSpinStart);
        EventManager.StartListening(EventNames.WheelSpinResult, WheelSpinResult);
        EventManager.StartListening(EventNames.ShopItemPurchased, ShopItemPurchased);
        EventManager.StartListening(EventNames.PerkItemPurchased, PerkItemPurchased);
        EventManager.StartListening(EventNames.NotEnoughMoney, NotEnoughMoney);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StopListening(EventNames.WheelSpinStart, WheelSpinStart);
        EventManager.StopListening(EventNames.WheelSpinResult, WheelSpinResult);
        EventManager.StopListening(EventNames.ShopItemPurchased, ShopItemPurchased);
        EventManager.StopListening(EventNames.PerkItemPurchased, PerkItemPurchased);
        EventManager.StopListening(EventNames.NotEnoughMoney, NotEnoughMoney);
    }
    public void HideShops() {
        ShopBackpacks.gameObject.SetActive(false);
        ShopHats.gameObject.SetActive(false);
        PerkShopList.gameObject.SetActive(false);
        UpgradeShop.gameObject.SetActive(false);
    }
}
