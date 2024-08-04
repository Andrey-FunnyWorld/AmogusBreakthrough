using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Wheel : MonoBehaviour {
    public WheelItem ItemOnWheelPrefab;
    public Transform WheelTransform;
    public AdSpinButton AdSpinButton;
    public ButtonScored FreeSpinButton;
    public float StartSpeed = 1440;
    public ButtonDisabled CloseButton;
    bool isSpinning = false;
    List<WheelItem> items;
    const float DURATION_EXTENT = 1;
    const int SECTOR_COUNT = 8;
    const int RARE_SECTOR_COUNT = 2;
    const int EPIC_SECTOR_COUNT = 1;
    const float SPIN_REWARD_DELAY = 0.5f;
    public ShopList BackpackShop, HatShop;
    public NewSkinPanel NewSkinPanel;
    public void Spin(float duration) {
        if (!isSpinning) {
            isSpinning = true;
            StartCoroutine(Spinning(GetDuration(duration)));
            EventManager.TriggerEvent(EventNames.WheelSpinStart, this);
            SetDisabledButtons(false);
        }
    }
    public void ApplyProgress(ProgressState state) {
        FreeSpinButton.Score = state.Spins;
        AdSpinButton.ApplyFinishDate(state.AdSpinWhenAvailable);
    }
    float GetDuration(float baseDuration) {
        return Random.Range(baseDuration - DURATION_EXTENT, baseDuration + DURATION_EXTENT);
    }
    IEnumerator Spinning(float duration) {
        float timer = 0;
        while (timer < duration) {
            timer += Time.deltaTime;
            float speed = StartSpeed * Utils.EaseInSquare(1 - timer / duration);
            WheelTransform.Rotate(0, 0, -speed / 360);
            yield return null;
        }
        isSpinning = false;
        HandleSpinResult(WheelTransform.rotation.eulerAngles.z);
    }
    void HandleSpinResult(float angle) {
        const float RANGE = 360 / SECTOR_COUNT;
        int sectorIndex = (int)Mathf.Abs(Mathf.Floor((360 - angle) / RANGE));
        EventManager.TriggerEvent(EventNames.WheelSpinResult, items[sectorIndex]);
        ShowReward(items[sectorIndex]);
        //Debug.Log(string.Format("Angle: {0}; Index: {1}; Type: {2}", angle, sectorIndex, items[sectorIndex].ItemType));
    }
    void ShowReward(WheelItem wheelItem) {
        int shopType = Random.Range(0, 2);
        ShopList shopList = shopType == 0 ? BackpackShop : HatShop;
        ListItem rewardItem = shopList.GetRandomItem(wheelItem.ItemType);
        if (rewardItem != null) {
            StartCoroutine(Utils.WaitAndDo(SPIN_REWARD_DELAY, () => {
                ShowRewardItem(rewardItem, shopList.ShopType);
            }));
        } else {
            shopList = shopList == HatShop ? BackpackShop : HatShop;
            rewardItem = shopList.GetRandomItem(wheelItem.ItemType);
            if (rewardItem != null) {
                rewardItem.Unlock(false);
                StartCoroutine(Utils.WaitAndDo(SPIN_REWARD_DELAY, () => {
                    ShowRewardItem(rewardItem, shopList.ShopType);
                }));
            } else {
                Debug.Log("No appropriate skins left");
            }
        }
    }
    void ShowRewardItem(ListItem rewardItem, SkinType shopType) {
        NewSkinPanel.ShowItem(rewardItem.Model, shopType);
        rewardItem.Unlock(false);
        SetDisabledButtons(true);
    }
    public void GenerateItems() {
        float radius = 0.7f * WheelTransform.GetComponent<RectTransform>().sizeDelta.x / 2;
        items = new List<WheelItem>(SECTOR_COUNT);
        for (int i = 0; i < SECTOR_COUNT; i++) {
            WheelItem item = Instantiate(ItemOnWheelPrefab, Vector2.zero, Quaternion.identity, WheelTransform);
            float deg = (i + 0.5f) * 360 / SECTOR_COUNT;
            if (i == 7) item.SetType(SkinItemQuality.Epic);
            else if (i == 6 || i == 0) item.SetType(SkinItemQuality.Rare);
            else item.SetType(SkinItemQuality.Regular);
            item.transform.localPosition = new Vector2(radius * Mathf.Cos(deg * Mathf.Deg2Rad), radius * Mathf.Sin(deg * Mathf.Deg2Rad));
            item.transform.Rotate(new Vector3(0, 0, deg));
            items.Add(item);
        }
    }
    void Start() {
        GenerateItems();
    }
    void OnEnable() {
        FreeSpinButton.Score = UserProgressController.Instance.ProgressState.Spins;
    }
    void SetDisabledButtons(bool enabled) {
        CloseButton.Enable = enabled;
        if (FreeSpinButton.Score > 0) FreeSpinButton.GetComponent<ButtonDisabled>().Enable = enabled;
        AdSpinButton.KeepDisabled = !enabled;
        if (enabled) {
            if (!AdSpinButton.IsTimerRunning) AdSpinButton.ButtonDisabled.Enable = true;
        }
        else
            AdSpinButton.ButtonDisabled.Enable = false;
    }
}