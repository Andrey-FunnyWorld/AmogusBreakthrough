using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    public Animator Transition;
    public float TransitionTime = 1;
    public const int MENU_BUILD_INDEX = 0;
    public const int BATTLE_BUILD_INDEX = 1;
    public static Difficulty Difficulty;
    //public AdsManager AdsManager;
    void Awake() {
        SubscribeEvents();
    }
    void OnDestroy() {
        UnsubscribeEvents();
    }
    public void LoadScene(int buildIndex) {
        Time.timeScale = 1;
        if (TransitionTime == 0)
            SceneManager.LoadScene(buildIndex);
        else {
            StartCoroutine(TransitionToLevel(buildIndex));
        }
    }
    IEnumerator TransitionToLevel(int buildIndex) {
        Transition.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(TransitionTime);
        SceneManager.LoadScene(buildIndex);
    }
    void LevelLoaded(object arg) {
        Transition.SetTrigger("End");
    }
    const int NON_LEVEL_SCENE_COUNT = 1;
    public int CurrentLevelIndex { get { 
        return SceneManager.GetActiveScene().buildIndex;
    }}
    public int LevelCount { get { return SceneManager.sceneCountInBuildSettings - NON_LEVEL_SCENE_COUNT; }}
    public void StartNextLevel() {
        // if (!ProgressController.Instance.ProgressState.SkipSaveTargetDialog && CurrentLevelIndex == 0) {
        //     ProgressController.Instance.ProgressState.SkipSaveTargetDialog = true;
        //     EventManager.StartListening(EventNames.AuthCompletedAndSave, AuthCompletedAndSave);
        //     EventManager.TriggerEvent(EventNames.SwitchUIScreen, new SwitchUIParameter() { Command = SwitchUICommand.AuthDialogOn, Source = this });
        // } else {
        //     if (AdsManager.InterstitialLevels.Contains(CurrentLevelIndex)) {
        //         AdsManager.ShowInterstitial(StartNextLevelCore);
        //     } else {
        //         StartNextLevelCore();
        //     }
        // }
    }
    void StartNextLevelCore() {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex < SceneManager.sceneCountInBuildSettings - NON_LEVEL_SCENE_COUNT - 1) {
            LoadScene(activeScene.buildIndex + 1);
        } else {
            //SceneManager.LoadScene("Final");
        }
    }
    public void RestartLevel() {
        Time.timeScale = 1;
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void SubscribeEvents() {
        EventManager.StartListening(EventNames.LevelLoaded, LevelLoaded);
    }
    void UnsubscribeEvents() {
        EventManager.StopListening(EventNames.LevelLoaded, LevelLoaded);
    }
     void AuthCompletedAndSave(object arg) {
        //EventManager.StopListening(EventNames.AuthCompletedAndSave, AuthCompletedAndSave);
        StartNextLevel();
    }
}
