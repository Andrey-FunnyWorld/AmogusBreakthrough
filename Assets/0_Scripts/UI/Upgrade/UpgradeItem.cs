using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeItem : MonoBehaviour, IPointerDownHandler {
    public TextMeshProUGUI PriceText, LevelText;
    public AudioSource AudioSource;
    [NonSerialized]
    public int Price;
    public UpgradeType UpgradeType;
    public Transform BuyButton;
    public int CurrentLevel = 0;
    public const int MAX_LEVEL = 10;
    int originalPrice;
    string format = "{0} <sprite name=\"coin\">";
    bool CanBuy() {
        return CurrentLevel < MAX_LEVEL;
    }
    public void OnPointerDown(PointerEventData arg) {
        if (CanBuy()) {
            EventManager.TriggerEvent(EventNames.UpgradeItemPurchaseTry, this);
        }
    }
    public void Upgrade() {
        SetLevel(CurrentLevel + 1);
        EventManager.TriggerEvent(EventNames.UpgradeItemPurchased, this);
        LevelText.text = CurrentLevel.ToString();
        AudioSource.Play();
    }
    void SetLevel(int newLevel) {
        CurrentLevel = newLevel;
        BuyButton.gameObject.SetActive(CanBuy());
    }
    public void ApplyProgress(ProgressState progress, int price) {
        originalPrice = price;
        Price = price;
        PriceText.text = string.Format(format, price);
        switch (UpgradeType) {
            case UpgradeType.AttackSpeed: SetLevel(progress.UpgradeLevelAttackSpeed); break;
            case UpgradeType.Damage: SetLevel(progress.UpgradeLevelDamage); break;
            case UpgradeType.HP: SetLevel(progress.UpgradeLevelHP); break;
        }
        LevelText.text = CurrentLevel.ToString();
    }
    public void MakeFree(bool free) {
        Price = free ? 0 : originalPrice;
        PriceText.text = free ? MyLocalization.Instance.GetLocalizedText(LocalizationKeys.ForFree) : string.Format(format, Price);
        BuyButton.gameObject.SetActive(CanBuy());
    }
}

public enum UpgradeType {
    HP, AttackSpeed, Damage
}