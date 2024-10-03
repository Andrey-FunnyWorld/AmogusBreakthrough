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
    public MenuCameraController cameraController;
    public CameraType[] Cameras;
    public AudioSource AudioSource;
    List<ListItem> liveItems = new List<ListItem>();
    const float collapsedTop = 310;
    ProgressState progressState;
    bool isCollapsed = false;
    int currentCameraView = 0;
    public void GenerateItems(ProgressState state) {
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
            UserProgressController.Instance.ProgressState.Equipp(ShopType, (SkinItemName)randomSkins[i], i + 1);
        }
        UserProgressController.Instance.SaveProgress();
        EventManager.TriggerEvent(EventNames.RandomSkins, 
            new RandomSkinArg() { RandomSkins = randomSkins.ToArray(), ShopType = ShopType }
        );
    }
    public void ToggleCameraView() {
        if (++currentCameraView >= Cameras.Length)
            currentCameraView = 0;
        cameraController.SwitchToCamera(Cameras[currentCameraView]);
        SetCollapsed(currentCameraView > 0);
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
        currentCameraView = 0;
    }
    void OnEnable() {
        SubscriveEvents();
        Grid.SetPageSize(10);
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.ShopItemPurchaseTry, ShopItemPurchaseTry);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.ShopItemPurchaseTry, ShopItemPurchaseTry);
    }
    void ShopItemPurchaseTry(object arg) {
        ShopItemPurchaseArgs args = (ShopItemPurchaseArgs)arg;
        if (args.ForFree) {
            UnlockItem(args.ItemModel);
        } else {
            bool enoughMoney = progressState.Money >= args.ItemModel.Price;
            if (enoughMoney) {
                AudioSource.Play();
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
    public ListItem GetRandomItem(SkinItemQuality quality) {
        int[] purchasedItems = ShopType == SkinType.Backpack ? progressState.PurchasedBackpacks : progressState.PurchasedHats;
        HashSet<int> purchasedSet = new HashSet<int>(purchasedItems);
        ShopItemModel[] availableItems = Items.Items.Where(i => !purchasedItems.Any(p => i.SkinName == (SkinItemName)p)).ToArray();
        if (availableItems.Length > 0) {
            availableItems = availableItems.Where(i => i.Quality == quality).ToArray();
            if (availableItems.Length > 0) {
                ShopItemModel reward = availableItems[Random.Range(0, availableItems.Length)];
                ListItem listItem = liveItems.FirstOrDefault(i => i.Model.SkinName == reward.SkinName);
                return listItem;
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