using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class PerkSelector : MonoBehaviour, IPointerDownHandler {
    public PerkStorage PerkStorage;
    public PerkItem PerkItemPrefab;
    public bool CanSelect = false;
    [NonSerialized]
    public PerkItem CurrentPerk;
    List<PerkItem> items;
    const float ITEM_SPACING = 30;
    PerkType[] availablePerks;
    public void CreatePerkItems(float itemHeight, PerkType[] unlockedPerks) {
        thisHeight = itemHeight;
        availablePerks = unlockedPerks;
        items = new List<PerkItem>(unlockedPerks.Length);
        foreach (PerkModel model in PerkStorage.Perks) {
            if (availablePerks.Contains(model.PerkType)) {
                PerkItem perkItem = Instantiate(PerkItemPrefab, transform);
                perkItem.Init(model);
                items.Add(perkItem);
                perkItem.RectTransform.anchoredPosition = new Vector2(0, (items.Count - 1) * (thisHeight + ITEM_SPACING));
            }
        }
    }
    void Start() {
    }
    public void RollToPerk(float duration, PerkType perkType) {
        rollingSpeed = (thisHeight + ITEM_SPACING) / ItemRollDuration;
        //int targetIndex = Array.IndexOf(PerkStorage.Perks.Select(p => p.PerkType).ToArray(), perkType);
        int targetIndex = Array.IndexOf(availablePerks, perkType);
        StartCoroutine(Rolling(duration, targetIndex));
    }
    float thisHeight;
    int currentIndex = 0;
    public float ItemRollDuration = 0.3f;
    float rollingSpeed;
    void RollItems() {
        for (int i = 0; i < items.Count; i++) {
            items[i].RectTransform.anchoredPosition += new Vector2(0, -rollingSpeed * Time.deltaTime);
            items[i].Alpha = 1 - MathF.Abs(items[i].RectTransform.anchoredPosition.y) / (ITEM_SPACING + thisHeight);
            if (items[i].RectTransform.anchoredPosition.y < 0) currentIndex = i + 1;
            if (items[i].RectTransform.anchoredPosition.y < -thisHeight - ITEM_SPACING) {
                int prevIndex = i - 1 < 0 ? availablePerks.Length - 1 : i - 1; 
                items[i].RectTransform.anchoredPosition = new Vector2(0, 
                    items[prevIndex].RectTransform.anchoredPosition.y + ITEM_SPACING + thisHeight
                );
            }
        }
        if (currentIndex >= items.Count) currentIndex = 0;
    }
    IEnumerator Rolling(float duration, int targetIndex) {
        float timer = 0;
        while (timer < duration) {
            timer += Time.deltaTime;
            RollItems();
            yield return null;
        }
        while (currentIndex != targetIndex) {
            RollItems();
            yield return null;
        }
        CurrentPerk = items[currentIndex];
        while (CurrentPerk.RectTransform.anchoredPosition.y > 0) {
            RollItems();
            yield return null;
        }
        CurrentPerk.SetTextVisibility(true);
        CurrentPerk.RectTransform.anchoredPosition = Vector2.zero;
        CurrentPerk.Alpha = 1;
        CanSelect = true;
    }
    public void OnPointerDown(PointerEventData e) {
        if (CanSelect) {
            EventManager.TriggerEvent(EventNames.PerkSelected, CurrentPerk);
        }
    }
}
