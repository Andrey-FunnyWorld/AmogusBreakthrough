using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PerkShopList : MonoBehaviour {
    public GridPaging Grid;
    public PerkListItem ListItemPrefab;
    [NonSerialized]
    public List<PerkListItem> liveItems = new List<PerkListItem>();
    public PerkStorage PerkStorage;
    ProgressState progressState;

    public void GenerateItems(ProgressState state) {
        progressState = state;
        int[] purchasedItems = state.PurchasedPerks;
        foreach (PerkModel model in PerkStorage.Perks) {
            CreateItem(model, purchasedItems.Contains((int)model.PerkType));
        }
        Grid.SetAllItems(liveItems.Select(i => i.transform).ToArray());
    }
    void CreateItem(PerkModel model, bool purchased) {
        PerkListItem item = Instantiate(ListItemPrefab);
        item.gameObject.SetActive(false);
        item.ApplyData(model, purchased);
        liveItems.Add(item);
    }
    void UnlockItem(PerkModel model) {
        PerkListItem listItem = liveItems.FirstOrDefault(i => i.Model.PerkType == model.PerkType);
        if (listItem != null) {
            listItem.Unlock();
        }
    }
    void PerkItemPurchaseTry(object arg) {
        PerkModel args = (PerkModel)arg;
        bool enoughMoney = progressState.Money >= args.Price;
        if (enoughMoney) {
            UnlockItem(args);
        }
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
        EventManager.StartListening(EventNames.PerkItemPurchaseTry, PerkItemPurchaseTry);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.PerkItemPurchaseTry, PerkItemPurchaseTry);
    }
}
