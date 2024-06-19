using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    public MainGuy MainGuy;
    public ShopList ShopBackpacks, ShopHats;
    public Transform MainMenu;
    public ButtonDisabled[] DisableWhenSpinning;
    public ProgressText3D HatText, BackpackText;
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
    }
    void UpdateProgressTexts(ProgressState progress) {
        HatText.SetProgress(CalcSkinProgress(SkinType.Hat, progress));
        BackpackText.SetProgress(CalcSkinProgress(SkinType.Backpack, progress));
    }
    void ShopItemPurchased(object arg) {
        ListItem listItem = (ListItem)arg;
        UserProgressController.Instance.ProgressState.AddPurchased(listItem.ShopType, listItem.Model.SkinName);
        UpdateProgressTexts(UserProgressController.Instance.ProgressState);
    }
    void StartDataLoaded(object arg) {
        ApplyProgress(UserProgressController.Instance.ProgressState);
        ShopBackpacks.GenerateItems(UserProgressController.Instance.ProgressState);
        ShopHats.GenerateItems(UserProgressController.Instance.ProgressState);
    }
    public void ShowShopAction(bool show) {
        MainMenu.gameObject.SetActive(!show);
    }
    List<ButtonDisabled> buttonsToSkip = new List<ButtonDisabled>();
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
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StopListening(EventNames.WheelSpinStart, WheelSpinStart);
        EventManager.StopListening(EventNames.WheelSpinResult, WheelSpinResult);
        EventManager.StopListening(EventNames.ShopItemPurchased, ShopItemPurchased);
    }
}
