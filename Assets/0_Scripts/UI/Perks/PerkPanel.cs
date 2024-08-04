using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PerkPanel : MonoBehaviour {
    public int PerkSelectorCount = 3;
    public PerkSelector PerkSelectorPrefab;
    public GridLayoutGroup Grid;
    public bool ShowOnStart = true;
    List<PerkSelector> selectors;
    const float ROLL_BASE_DURATION = 2f;
    const float ROLL_DURATION_OFFSET = 1f;
    void CreateSelectors() {
        selectors = new List<PerkSelector>(PerkSelectorCount);
        for (int i = 0; i < PerkSelectorCount; i++) {
            PerkSelector selector = Instantiate(PerkSelectorPrefab, Grid.transform);
            selector.CreatePerkItems(Grid.cellSize.y);
            selector.ItemRollDuration = Random.Range(0.1f, 0.2f);
            selectors.Add(selector);
        }
    }
    void Start() {
        gameObject.SetActive(ShowOnStart);
        if (ShowOnStart) {
            SubscriveEvents();
            CreateSelectors();
            RollSelectors();
        }
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
    public void RollSelectors() {
        PerkType[] perks = GetRandomPerks1(PerkSelectorCount);
        for (int i = 0; i < PerkSelectorCount; i++) {
            selectors[i].RollToPerk(ROLL_BASE_DURATION + ROLL_DURATION_OFFSET * i, perks[i]);
            Debug.Log(perks[i]);
        }
    }
    PerkType[] GetRandomPerks(int perkCount) {
        PerkType[] perks = new PerkType[perkCount];
        List<PerkType> availablePerks = PerkSelectorPrefab.PerkStorage.Perks.Select(p => p.PerkType).ToList();
        for (int i = 0; i < perkCount; i++) {
            perks[i] = availablePerks[Random.Range(0, availablePerks.Count)];
            availablePerks.Remove(perks[i]);
        }
        return perks;
    }
    PerkType[] GetRandomPerks1(int perkCount) {
        PerkType[] perks = new PerkType[perkCount];
        List<PerkType> availablePerks = PerkSelectorPrefab.PerkStorage.Perks.Select(p => p.PerkType).ToList();
        perks[0] = PerkType.OnePunchKill;
        for (int i = 1; i < perkCount; i++) {
            perks[i] = availablePerks[Random.Range(0, availablePerks.Count)];
            availablePerks.Remove(perks[i]);
        }
        return perks;
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.PerkSelected, PerkSelected);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.PerkSelected, PerkSelected);
    }
    void PerkSelected(object arg) {
        gameObject.SetActive(false);
    }
}
