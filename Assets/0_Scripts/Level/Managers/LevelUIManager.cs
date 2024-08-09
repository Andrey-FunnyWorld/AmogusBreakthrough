using UnityEngine;

public class LevelUIManager : MonoBehaviour {
    public Transform StartupMsg;
    public Transform RoadFinishedMsg;

    public AbilityButton FirstAbility;
    public AbilityButton SecondAbility;

    public void LetsRoll() {
        //StartupMsg.gameObject.SetActive(false);
    }

    public void RoadFinished() {
        //RoadFinishedMsg.gameObject.SetActive(true);
    }

    public void HandlePerk(PerkItem perk) {
        if (!FirstAbility.gameObject.activeSelf) {
            FirstAbility.Init(perk);
        } else {
            SecondAbility.Init(perk);
        }
    }

    public void HandlePerk(PerkType type, Sprite sprite) {
        if (!FirstAbility.gameObject.activeSelf) {
            FirstAbility.Init(type, sprite);
        } else {
            SecondAbility.Init(type, sprite);
        }
    }

}
