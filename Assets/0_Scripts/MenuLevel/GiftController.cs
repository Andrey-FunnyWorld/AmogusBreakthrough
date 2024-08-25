using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GiftController : MonoBehaviour {
    public float OpenDelay = 2.5f;
    public AudioSource AudioSource;
    public AudioClip Drums, OpenSound;
    public NewSkinPanel NewSkinPanel;
    public GiftRewardPanel GiftPanel;
    public ShopList BackpackShop, HatShop;
    public UpgradeShop UpgradeShop;
    public PerkShopList PerkShopList;
    public float GraySkinWeight = 3;
    public float GreenSkinWeight = 2;
    public float BlueSkinWeight = 1;
    public float DiamondWeight = 2;
    public float UpgradeWeight = 2;
    public float PerkWeight = 1;
    public float MoneyWeight = 2;
    Dictionary<GiftType, float> chances;
    void Start() {
        CalcWeights();
    }
    void CalcWeights() {
        float sum = GraySkinWeight + GreenSkinWeight + BlueSkinWeight + DiamondWeight + UpgradeWeight + PerkWeight + MoneyWeight;
        chances = new Dictionary<GiftType, float>() {
            { GiftType.GraySkin, GraySkinWeight / sum },
            { GiftType.GreenSkin, GreenSkinWeight / sum },
            { GiftType.BlueSkin, BlueSkinWeight / sum },
            { GiftType.Diamond, DiamondWeight / sum },
            { GiftType.Upgrade, UpgradeWeight / sum },
            { GiftType.Perk, PerkWeight / sum },
            { GiftType.Money, MoneyWeight / sum },
        };
    }
    void CheckWeights() {
        if (UpgradeWeight > 0 && UpgradeShop.DamageItem.CurrentLevel == UpgradeItem.MAX_LEVEL 
            && UpgradeShop.HPItem.CurrentLevel == UpgradeItem.MAX_LEVEL
            && UpgradeShop.AttackSpeedItem.CurrentLevel == UpgradeItem.MAX_LEVEL) {
                UpgradeWeight = 0;
                CalcWeights();
            }
        if (PerkWeight > 0 && !PerkShopList.liveItems.Any(p => !p.IsPurchased)) {
            PerkWeight = 0;
            CalcWeights();
        }
        if (GraySkinWeight > 0 && BackpackShop.GetRandomItem(SkinItemQuality.Regular) == null && HatShop.GetRandomItem(SkinItemQuality.Regular) == null) {
            GraySkinWeight = 0;
            CalcWeights();
        }
        if (GreenSkinWeight > 0 && BackpackShop.GetRandomItem(SkinItemQuality.Rare) == null && HatShop.GetRandomItem(SkinItemQuality.Rare) == null) {
            GreenSkinWeight = 0;
            CalcWeights();
        }
        if (BlueSkinWeight > 0 && BackpackShop.GetRandomItem(SkinItemQuality.Epic) == null && HatShop.GetRandomItem(SkinItemQuality.Epic) == null) {
            BlueSkinWeight = 0;
            CalcWeights();
        }
    }
    public GiftType GetRandomGiftType() {
        CheckWeights();
        float random = Random.Range(0f, 1f);
        float stack = 0;
        foreach (KeyValuePair<GiftType, float> pair in chances) {
            if (random >= stack && random < stack + pair.Value) {
                return pair.Key;
            } else stack += pair.Value;
        }
        return GiftType.GraySkin;
    }
    public void GetGift(UnityAction openAction = null) {
        AudioSource.clip = Drums;
        AudioSource.loop = true;
        AudioSource.Play();
        StartCoroutine(Utils.WaitAndDo(OpenDelay, () => {
            AudioSource.Stop();
            AudioSource.loop = false;
            AudioSource.clip = OpenSound;
            AudioSource.Play();
            GiftType giftType = GetRandomGiftType();
            if (openAction != null)
                openAction.Invoke();
            if (giftType.ToString().Contains("Skin")) {
                SkinItemQuality quality = giftType == GiftType.GraySkin ? SkinItemQuality.Regular : giftType == GiftType.GreenSkin ? SkinItemQuality.Rare : SkinItemQuality.Epic;
                ListItem skinItem = GetRandomSkin(quality);
                NewSkinPanel.ShowItem(skinItem.Model, skinItem.ShopType);
                skinItem.Unlock(false, true);
            } else {
                GiftPanel.Show(giftType);
            }
        }));
    }
    ListItem GetRandomSkin(SkinItemQuality quality) {
        int shopType = Random.Range(0, 2);
        ShopList shopList = shopType == 0 ? BackpackShop : HatShop;
        ListItem rewardItem = shopList.GetRandomItem(quality);
        return rewardItem;
    }
}

public enum GiftType {
    GraySkin, GreenSkin, BlueSkin, Diamond, Upgrade, Perk, Money
}