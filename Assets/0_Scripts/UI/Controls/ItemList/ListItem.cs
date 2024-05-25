using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

public class ListItem : MonoBehaviour, IPointerDownHandler {
    public Image ItemImage, QualityImage;
    public TextMeshProUGUI PriceText;
    public Transform Buy, Equip;
    public SkinQualityColors SkinQualityColors;
    [NonSerialized]
    public bool IsAvaiable = false;
    [NonSerialized]
    public ShopItemModel Model;
    [NonSerialized]
    public ShopType ShopType;
    string priceFormat = "{0} <sprite name=\"coin\">";
    public void ApplyData(ShopItemModel model, bool purchased, ShopType shopType) {
        PriceText.text = string.Format(priceFormat, model.Price);
        ItemImage.sprite = model.Sprite;
        ApplyPurchased(purchased);
        Model = model;
        ShopType = shopType;
        QualityImage.color = SkinQualityColors.Colors.First(c => c.Quality == model.Quality).Color;
    }
    void ApplyPurchased(bool purchased) {
        Equip.gameObject.SetActive(purchased);
        Buy.gameObject.SetActive(!purchased);
        IsAvaiable = purchased;
    }
    public void Unlock() {
        ApplyPurchased(true);
    }
    public void OnPointerDown(PointerEventData arg) {
        if (IsAvaiable) {
            EventManager.TriggerEvent(EventNames.SkinItemEquip, new SkinItemEquipArgs() { ItemModel = Model, ShopType = ShopType });
        } else {
            EventManager.TriggerEvent(EventNames.ShopItemPurchaseTry, new ShopItemPurchaseArgs() { ItemModel = Model, ForFree = false });
        }
    }
}

public class SkinItemEquipArgs {
    public ShopType ShopType;
    public ShopItemModel ItemModel;
}
public class ShopItemPurchaseArgs {
    public ShopItemModel ItemModel;
    public bool ForFree;
}