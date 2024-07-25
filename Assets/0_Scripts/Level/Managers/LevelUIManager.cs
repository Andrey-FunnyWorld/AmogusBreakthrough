using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour {
    public Transform StartupMsg;
    public Transform RoadFinishedMsg;
    public Button AbilityButton;
    public Image HpBar;

    PerkType? currentAbility;

    void Awake() {
        SubscribeEvents();
        AbilityButton.onClick.RemoveAllListeners();
        AbilityButton.onClick.AddListener(() => UseAbility());
        AbilityButton.gameObject.SetActive(false);
    }

    void OnDestroy() => UnsubscribeEvents();

    public void LetsRoll() =>
        StartupMsg.gameObject.SetActive(false);

    public void RoadFinished() =>
        RoadFinishedMsg.gameObject.SetActive(true);

    public void HandlePerk(PerkType perk) {
        currentAbility = perk;
        //button with ability (change sprite?)
        if (!AbilityButton.gameObject.activeSelf)
            AbilityButton.gameObject.SetActive(true);
    }

    void UpdateHP(object currentHp) =>
        HpBar.fillAmount = (float)currentHp;

    void UseAbility() {
        if (currentAbility == null)
            return;

        if (currentAbility == PerkType.OnePunchKill) {
            EventManager.TriggerEvent(EventNames.AbilityOnePunch);
        } else if (currentAbility == PerkType.Bubble) {
            EventManager.TriggerEvent(EventNames.AbilityBubble);
        }
        currentAbility = null;
        AbilityButton.gameObject.SetActive(false);
    }

    void SubscribeEvents() =>
        EventManager.StartListening(EventNames.HpChanged, UpdateHP);

    void UnsubscribeEvents() =>
        EventManager.StopListening(EventNames.HpChanged, UpdateHP);
}
