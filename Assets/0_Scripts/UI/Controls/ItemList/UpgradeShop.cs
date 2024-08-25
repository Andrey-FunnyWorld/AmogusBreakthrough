using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShop : MonoBehaviour {
    ProgressState progressState;
    public UpgradeItem AttackSpeedItem, HPItem, DamageItem;
    public int UpgradePrice = 100;
    public Transform CloseButton;
    bool nextForFree = false;
    public void ApplyProgress(ProgressState progress) {
        progressState = progress;
        AttackSpeedItem.ApplyProgress(progress, UpgradePrice);
        HPItem.ApplyProgress(progress, UpgradePrice);
        DamageItem.ApplyProgress(progress, UpgradePrice);
    }
    public void UpgradeItemPurchaseTry(object arg) {
        UpgradeItem item = (UpgradeItem)arg;
        bool enoughMoney = progressState.Money >= item.Price;
        if (enoughMoney) {
            item.Upgrade();
            SetFreeMode(false);
        } else {
            EventManager.TriggerEvent(EventNames.NotEnoughMoney, item);
        }
    }
    public void SetFreeMode(bool free) {
        nextForFree = free;
        AttackSpeedItem.MakeFree(free);
        HPItem.MakeFree(free);
        DamageItem.MakeFree(free);
        CloseButton.gameObject.SetActive(!free);
    }
    void OnDisable() {
        UnsubscriveEvents();
    }
    void OnEnable() {
        SubscriveEvents();
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.UpgradeItemPurchaseTry, UpgradeItemPurchaseTry);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.UpgradeItemPurchaseTry, UpgradeItemPurchaseTry);
    }
}
