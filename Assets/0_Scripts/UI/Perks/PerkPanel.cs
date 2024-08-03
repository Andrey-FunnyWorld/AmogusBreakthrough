using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PerkPanel : MonoBehaviour {
    public int PerkSelectorCount = 3;
    public PerkSelector PerkSelectorPrefab;
    public GridLayoutGroup Grid;
    public Transform Buttons;
    public bool ShowOnStart = true;
    [System.NonSerialized]
    public bool ExtraPerkTaken = false;
    public int ExtraRollPrice = 100;
    public TMPro.TextMeshProUGUI ExtraRollText, YourCoinsText;
    PerkType? takenExtraPerk = null;
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
            ExtraRollText.text = string.Format(
                MyLocalization.Instance.GetLocalizedText(LocalizationKeys.ExtraPerkFormat), ExtraRollPrice
            );
        }
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
    public void RollSelectors() {
        PerkType[] perks = GetRandomPerks(PerkSelectorCount, takenExtraPerk);
        for (int i = 0; i < PerkSelectorCount; i++) {
            selectors[i].RollToPerk(ROLL_BASE_DURATION + ROLL_DURATION_OFFSET * i, perks[i]);
            Debug.Log(perks[i]);
        }
    }
    PerkType[] GetRandomPerks(int perkCount, PerkType? perkType) {
        PerkType[] perks = new PerkType[perkCount];
        List<PerkType> availablePerks = PerkSelectorPrefab.PerkStorage.Perks.Select(p => p.PerkType).Where(p => p != perkType).ToList();
        for (int i = 0; i < perkCount; i++) {
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
    public void ExtraPerkRoll() {
        Buttons.gameObject.SetActive(false);
        Grid.gameObject.SetActive(true);
        ExtraPerkTaken = true;
        RollSelectors();
        UserProgressController.Instance.ProgressState.Money -= ExtraRollPrice;
        UserProgressController.Instance.SaveProgress();
    }
    void PerkSelected(object arg) {
        if (ExtraPerkTaken)
            gameObject.SetActive(false);
        else {
            Grid.gameObject.SetActive(false);
            Buttons.gameObject.SetActive(true);
            PerkItem perkItem = (PerkItem)arg;
            takenExtraPerk = perkItem.PerkType;
        }
    }
    public void ApplyProgress(ProgressState state) {
        YourCoinsText.text = string.Format(
            MyLocalization.Instance.GetLocalizedText(LocalizationKeys.YouHaveCoins), state.Money
        );
        ExtraRollText.transform.parent.GetComponent<ButtonDisabled>().Enable = state.Money >= ExtraRollPrice;
    }
}
