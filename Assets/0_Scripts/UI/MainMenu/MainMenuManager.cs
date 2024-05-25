using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    public MainGuy MainGuy;
    public ShopList ShopBackpacks, ShopHats;
    public Transform MainMenu;
    public ButtonDisabled[] DisableWhenSpinning;
    void Start() {
        SubscriveEvents();
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
    void ApplyProgress(ProgressState progress) {
        MainGuy.ApplyProgress(progress);
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
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StopListening(EventNames.WheelSpinStart, WheelSpinStart);
        EventManager.StopListening(EventNames.WheelSpinResult, WheelSpinResult);
    }
}
