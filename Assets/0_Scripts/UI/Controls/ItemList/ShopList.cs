using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ShopList : MonoBehaviour {
    public Transform Grid;
    public ListItem ListItemPrefab;
    public SkinItems Items;
    public ShopType ShopType;
    List<ListItem> liveItems = new List<ListItem>();
    const float collapsedTop = 310;
    public void GenerateItems(ProgressState state) {
        int[] purchasedItems = ShopType == ShopType.Backpack ? state.PurchasedBackpacks : state.PurchasedHats; 
        foreach (ShopItemModel model in Items.Items) {
            CreateItem(model, purchasedItems.Contains((int)model.SkinName));
        }
    }
    void CreateItem(ShopItemModel model, bool purchased) {
        ListItem item = Instantiate(ListItemPrefab, Grid);
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
    }
    public void SetCollapsed(bool collapse) {
        isCollapsed = collapse;
        RectTransform rectTr = GetComponent<RectTransform>();
        rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, collapse ? -collapsedTop : 0);
        rectTr.anchoredPosition = new Vector2(rectTr.anchoredPosition.x, collapse ? (rectTr.sizeDelta.y / 2) : 0);
    }
    void OnDisable() {
        SetCollapsed(false);
    }
}

public enum ShopType {
    Backpack, Hat
}

public class RandomSkinArg {
    public int[] RandomSkins;
    public ShopType ShopType;
}