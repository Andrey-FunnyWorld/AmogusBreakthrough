using System.Collections;
using UnityEngine;

public class LevelUIManager : MonoBehaviour {
    public LevelManager LevelManager;
    public DefeatUI DefeatUI;
    public BattleTutorial BattleTutorial;
    Coroutine waitTutorialRoutine = null;
    public AbilityButton FirstAbility;
    public AbilityButton SecondAbility;
    public Transform BattleStats;
    public Transform HpBar;

    public interface IPlatformAdaptable {
        void Adapt(PlatformType platformType);
    }

    void Start() {
        waitTutorialRoutine = StartCoroutine(WaitForTutorial(BattleTutorial.ShowDelay));
    }

    void Update() {
        // if (Input.GetKeyDown(KeyCode.B)) {
        //     DefeatViewModel vm = new DefeatViewModel() { CoinReward = 70 };
        //     DefeatUI.ShowResult(vm);
        // }
    }

    void OnDestroy() {
        UnsubscribeEvents();
    }

    public void AdjustToPlatform(PlatformType platformType) {
        IPlatformAdaptable[] adaptables = GetComponentsInChildren<IPlatformAdaptable>(true);
        foreach (IPlatformAdaptable adaptable in adaptables)
            adaptable.Adapt(platformType);
    }

    public void RoadFinished() {
        HpBar.gameObject.SetActive(false);
        //RoadFinishedMsg.gameObject.SetActive(true);
    }

    public void GameOver(int coins) {
        DefeatUI.ShowResult(new DefeatViewModel() { CoinReward = coins });
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

    void StartMovement(object arg) {
        if (waitTutorialRoutine != null)
            StopCoroutine(waitTutorialRoutine);
        BattleTutorial.gameObject.SetActive(false);
    }

    IEnumerator WaitForTutorial(float delay) {
        while (LevelManager.MovementController == null || !LevelManager.MovementController.AllowMove) {
            yield return null;
        }
        SubscribeEvents();
        BattleStats.gameObject.SetActive(true);
        float timer = 0;
        while (timer < delay) {
            timer += Time.deltaTime;
            yield return null;
        }
        BattleTutorial.gameObject.SetActive(true);
    }

    void SubscribeEvents() {
        EventManager.StartListening(EventNames.StartMovement, StartMovement);
    }
    void UnsubscribeEvents() {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
    }

}
