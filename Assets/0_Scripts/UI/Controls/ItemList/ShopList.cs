using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ShopList : MonoBehaviour {
    public GridPaging Grid;
    public ListItem ListItemPrefab;
    public SkinItems Items;
    public SkinType ShopType;
    public Wheel Wheel;
    List<ListItem> liveItems = new List<ListItem>();
    const float collapsedTop = 310;
    ProgressState progressState;
    public NewSkinPanel NewSkinPanel;
    public void GenerateItems(ProgressState state) {
        Wheel.ApplyProgress(state);
        progressState = state;
        int[] purchasedItems = ShopType == SkinType.Backpack ? state.PurchasedBackpacks : state.PurchasedHats; 
        foreach (ShopItemModel model in Items.Items) {
            CreateItem(model, purchasedItems.Contains((int)model.SkinName));
        }
        Grid.SetAllItems(liveItems.Select(i => i.transform).ToArray());
    }
    void CreateItem(ShopItemModel model, bool purchased) {
        ListItem item = Instantiate(ListItemPrefab);
        item.gameObject.SetActive(false);
        item.ApplyData(model, purchased, ShopType);
        liveItems.Add(item);
    }
    public void RandomEquip() {
        int[] availableSkins = liveItems.Where(i => i.IsAvaiable).Select(i => (int)i.Model.SkinName).ToArray();
        List<int> randomSkins = new List<int>(Team.MAX_CAPACITY);
        for (int i = 0; i < Team.MAX_CAPACITY; i++) {
            randomSkins.Add(availableSkins[Random.Range(0, availableSkins.Length)]);
        }
        EventManager.TriggerEvent(EventNames.RandomSkins, 
            new RandomSkinArg() { RandomSkins = randomSkins.ToArray(), ShopType = ShopType }
        );
    }
    bool isCollapsed = false;
    public void ToggleCollapsed() {
        SetCollapsed(!isCollapsed);
        Grid.SetPageSize(isCollapsed ? 2 : 10);
    }
    public void SetCollapsed(bool collapse) {
        isCollapsed = collapse;
        RectTransform rectTr = GetComponent<RectTransform>();
        rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, collapse ? -collapsedTop : 0);
        rectTr.anchoredPosition = new Vector2(rectTr.anchoredPosition.x, collapse ? (rectTr.sizeDelta.y / 2) : 0);
    }
    void OnDisable() {
        SetCollapsed(false);
        UnsubscriveEvents();
    }
    void OnEnable() {
        SubscriveEvents();
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.WheelSpinResult, WheelSpinResult);
        EventManager.StartListening(EventNames.ShopItemPurchaseTry, ShopItemPurchaseTry);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.WheelSpinResult, WheelSpinResult);
        EventManager.StopListening(EventNames.ShopItemPurchaseTry, ShopItemPurchaseTry);
    }
    const float SPIN_REWARD_DELAY = 0.5f;
    void WheelSpinResult(object arg) {
        WheelItem wheelItem = (WheelItem)arg;
        ShopItemModel rewardItem = GetRandomItem(wheelItem.ItemType);
        if (rewardItem != null) {
            StartCoroutine(Utils.WaitAndDo(SPIN_REWARD_DELAY, () => NewSkinPanel.ShowItem(rewardItem, ShopType) ));
        } else {
            Debug.Log("No appropriate skins left");
        }
    }
    void ShopItemPurchaseTry(object arg) {
        ShopItemPurchaseArgs args = (ShopItemPurchaseArgs)arg;
        if (args.ForFree) {
            UnlockItem(args.ItemModel);
        } else {
            bool enoughMoney = progressState.Money >= args.ItemModel.Price;
            if (enoughMoney) {
                UnlockItem(args.ItemModel);
            } else {
                EventManager.TriggerEvent(EventNames.NotEnoughMoney, args);
            }
        }
    }
    void UnlockItem(ShopItemModel model) {
        ListItem listItem = liveItems.FirstOrDefault(i => i.Model.SkinName == model.SkinName);
        if (listItem != null) {
            listItem.Unlock();
        }
    }
    ShopItemModel GetRandomItem(SkinItemQuality quality) {
        int[] purchasedItems = ShopType == SkinType.Backpack ? progressState.PurchasedBackpacks : progressState.PurchasedHats;
        HashSet<int> purchasedSet = new HashSet<int>(purchasedItems);
        ShopItemModel[] availableItems = Items.Items.Where(i => !purchasedItems.Any(p => i.SkinName == (SkinItemName)p)).ToArray();
        if (availableItems.Length > 0) {
            availableItems = availableItems.Where(i => i.Quality == quality).ToArray();
            if (availableItems.Length > 0) {
                ShopItemModel reward = availableItems[Random.Range(0, availableItems.Length)];
                return reward;
            }
        }
        return null;
    }
}

public enum SkinType {
    Backpack, Hat
}

public class RandomSkinArg {
    public int[] RandomSkins;
    public SkinType ShopType;
}