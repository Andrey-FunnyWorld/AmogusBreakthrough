using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ListItem : MonoBehaviour, IPointerDownHandler {
    public Image ItemImage;
    public TextMeshProUGUI PriceText;
    public Transform Buy, Equip;
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
    }
    void ApplyPurchased(bool purchased) {
        Equip.gameObject.SetActive(purchased);
        Buy.gameObject.SetActive(!purchased);
        IsAvaiable = purchased;
    }
    public void Purchase() {
        ApplyPurchased(true);
    }
    public void OnPointerDown(PointerEventData arg) {
        EventManager.TriggerEvent(EventNames.ShopItemClick, this);
    }
}