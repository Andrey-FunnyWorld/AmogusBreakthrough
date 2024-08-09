using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public KeyCode ButtonKey;
    public Image Image;

    PerkType? currentPerk;

    public void Init(PerkItem perkItem) {
        Image.sprite = perkItem.PerkImage.sprite;
        currentPerk = perkItem.PerkType;
        gameObject.SetActive(true);
    }

    public void Init(PerkType type, Sprite sprite) {
        Image.sprite = sprite;
        currentPerk = type;
        gameObject.SetActive(true);
    }

    void Update() {
        if (Input.GetKeyDown(ButtonKey))
            UseAbility();
    }

    public void UseAbility() {
        gameObject.SetActive(false);
        EventManager.TriggerEvent(GetTriggerEventName());
        currentPerk = null;
    }

    string GetTriggerEventName() {
        if (currentPerk == PerkType.OnePunchKill) {
            return EventNames.AbilityOnePunch;
        } else if (currentPerk == PerkType.Bubble) {
            return EventNames.AbilityBubble;
        } else
            return null;
    }
}
