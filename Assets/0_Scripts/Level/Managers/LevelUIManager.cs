using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class LevelUIManager : MonoBehaviour {
    public LevelManager LevelManager;
    public DefeatUI DefeatUI;
    public BattleTutorial BattleTutorial;
    Coroutine waitTutorialRoutine = null;
    void Update() {
        // if (Input.GetKeyDown(KeyCode.B)) {
        //     DefeatViewModel vm = new DefeatViewModel() { CoinReward = 70 };
        //     DefeatUI.ShowResult(vm);
        // }
    }
    void Start() {
        waitTutorialRoutine = StartCoroutine(WaitForTutorial(BattleTutorial.ShowDelay));
    }
    IEnumerator WaitForTutorial(float delay) {
        while (LevelManager.MovementController == null || !LevelManager.MovementController.AllowMove) {
            yield return null;
        }
        SubscribeEvents();
        float timer = 0;
        while (timer < delay) {
            timer += Time.deltaTime;
            yield return null;
        }
        BattleTutorial.gameObject.SetActive(true);
    }
    public void AdjustToPlatform(PlatformType platformType) {
        IPlatformAdaptable[] adaptables = GetComponentsInChildren<IPlatformAdaptable>(true);
        foreach (IPlatformAdaptable adaptable in adaptables)
            adaptable.Adapt(platformType);
    }
    void StartMovement(object arg) {
        if (waitTutorialRoutine != null)
            StopCoroutine(waitTutorialRoutine);
        BattleTutorial.gameObject.SetActive(false);
    }
    void SubscribeEvents() {
        EventManager.StartListening(EventNames.StartMovement, StartMovement);
    }
    void UnsubscribeEvents() {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
    }
    void OnDestroy() {
        UnsubscribeEvents();
    }
}
public interface IPlatformAdaptable {
    void Adapt(PlatformType platformType);
}