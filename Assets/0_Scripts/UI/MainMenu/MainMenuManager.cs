using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    public MainGuy MainGuy;
    public ShopList ShopBackpacks, ShopHats;
    public Transform MainMenu;
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
    void ShopItemClick(object arg) {
        ListItem item = (ListItem)arg;
        if (!item.IsAvaiable) {
            // check if money is enough
            item.Purchase();
        }
    }
    public void ShowShopAction(bool show) {
        MainMenu.gameObject.SetActive(!show);
    }
    // void ShopListCloseButtonClick(object arg) {
    //     ShowShopAction(false);
    // }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StartListening(EventNames.ShopItemClick, ShopItemClick);
        //EventManager.StartListening(EventNames.ShopListCloseButtonClick, ShopListCloseButtonClick);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.StartDataLoaded, StartDataLoaded);
        EventManager.StopListening(EventNames.ShopItemClick, ShopItemClick);
        //EventManager.StopListening(EventNames.ShopListCloseButtonClick, ShopListCloseButtonClick);
    }
}
