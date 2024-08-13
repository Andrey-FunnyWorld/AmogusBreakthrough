using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PerkListItem : MonoBehaviour, IPointerDownHandler {
    public Image Image;
    public AudioSource AudioSource;
    public TextMeshProUGUI PriceText, NameText, DescriptionText;
    public Transform Buy, Available;
    [NonSerialized]
    public bool IsPurchased;
    string priceFormat = "{0} <sprite name=\"coin\">";
    public PerkModel Model;
    public void ApplyData(PerkModel model, bool purchased) {
        Image.sprite = model.Sprite;
        ApplyPurchased(purchased);
        Model = model;
        PriceText.text = string.Format(priceFormat, model.Price);
        NameText.text = model.Name;
        DescriptionText.text = model.Description;
    }
    void ApplyPurchased(bool purchased) {
        IsPurchased = purchased;
        Buy.gameObject.SetActive(!purchased);
        Available.gameObject.SetActive(purchased);
    }
    public void Unlock() {
        ApplyPurchased(true);
        EventManager.TriggerEvent(EventNames.PerkItemPurchased, Model);
        AudioSource.Play();
    }
    public void OnPointerDown(PointerEventData arg) {
        if (!IsPurchased) {
            EventManager.TriggerEvent(EventNames.PerkItemPurchaseTry, Model);
        }
    }
}
