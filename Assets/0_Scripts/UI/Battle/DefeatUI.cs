using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatUI : MonoBehaviour {
    public ScoreText CoinText;
    public LevelLoader LevelLoader;
    public Transform AdButton;
    bool canLoadNextLevel = true;
    DefeatViewModel viewModel;
    public void AdRewardCoins(int multiplier) {
        canLoadNextLevel = false;
        HtmlBridge.Instance.ShowRewarded(() => {
            viewModel.CoinReward *= multiplier;
            SetCoins();
            canLoadNextLevel = true;
        }, () => {
            canLoadNextLevel = true;
        });
    }
    public void ShowResult(DefeatViewModel vm) {
        viewModel = vm;
        gameObject.SetActive(true);
        AdButton.gameObject.SetActive(UserProgressController.Instance.ProgressState.CompletedRoundsCount > 0);
        SetCoins();
    }
    public void SetCoins() {
        CoinText.Score = viewModel.CoinReward;
    }
    public void FinishLevel() {
        if (canLoadNextLevel) {
            if (UserProgressController.Instance.ProgressState.CompletedRoundsCount > 0)
                UserProgressController.Instance.SaveProgress();
            HtmlBridge.Instance.ShowInterstitial(() => {
                gameObject.SetActive(false);
                LevelLoader.LoadScene(LevelLoader.MENU_BUILD_INDEX);
            });
            #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN
            LevelLoader.LoadScene(LevelLoader.MENU_BUILD_INDEX);
            #endif
        }
    }
}

public class DefeatViewModel {
    public int CoinReward;
}