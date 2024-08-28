using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResultUI : MonoBehaviour {
    public ImposterResultUI ImposterResultUI;
    public ScoreText CoinText, DiamondText;
    public Transform ImposterDetected, ImposterNotDetected, ResultPanel;
    public Animator Animator;
    public UnityEvent[] AnimationSteps;
    public LevelLoader LevelLoader;
    public MyDialog LoginDialog, RateDialog;
    ResultUIViewModel viewModel;
    bool canLoadNextLevel = true;
    public void ShowImposterResult(bool success) {
        ImposterResultUI.ShowResult(success);
    }
    public void ShowResult(ResultUIViewModel vm) {
        viewModel = vm;
        ResultPanel.gameObject.SetActive(true);
        ImposterDetected.gameObject.SetActive(false);
        ImposterNotDetected.gameObject.SetActive(false);
        Animator.SetTrigger("show");
        UserProgressController.Instance.ProgressState.CompletedRoundsCount++;
        HtmlBridge.Instance.ReportMetric(MetricNames.Win);
        LoserAssistant.RoundsPlayed++;
    }
    public void SetCoins() {
        CoinText.Score = viewModel.CoinReward;
    }
    public void SetDiamonds() {
        DiamondText.Score = viewModel.DiamondReward;
    }
    public void SetImposterStatus() {
        ImposterDetected.gameObject.SetActive(viewModel.ImposterDetected);
        ImposterNotDetected.gameObject.SetActive(!viewModel.ImposterDetected);
    }
    public void SetAnimationStep(int index) {
        AnimationSteps[index].Invoke();
    }
    public void FinishLevel() {
        if (!UserProgressController.Instance.ProgressState.SkipSaveTargetDialog) {
            UserProgressController.Instance.ProgressState.SkipSaveTargetDialog = true;
            LoginDialog.Show(() => {
                HtmlBridge.Instance.AskToLogin();
            }, () => {
                LoadMenuLevel();
            });
        } else {
            LoadMenuLevel();
        }
    }
    public void LoadMenuLevel() {
        if (canLoadNextLevel) {
            gameObject.SetActive(false);
            ProgressState state = UserProgressController.Instance.ProgressState;
            if (!state.AskedForRating && state.CompletedRoundsCount >= 3 && viewModel.ImposterDetected) {
                state.AskedForRating = true;
                RateDialog.Show(() => {
                    HtmlBridge.Instance.RateGame();
                    LevelLoader.LoadScene(LevelLoader.MENU_BUILD_INDEX);
                }, () => {
                    HtmlBridge.Instance.ShowInterstitial(() => {
                        LevelLoader.LoadScene(LevelLoader.MENU_BUILD_INDEX);
                    });
                });
            } else {
                HtmlBridge.Instance.ShowInterstitial(() => {
                    LevelLoader.LoadScene(LevelLoader.MENU_BUILD_INDEX);
                });
            }
            AddProgress();
            UserProgressController.Instance.SaveProgress();
            #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN
            LevelLoader.LoadScene(LevelLoader.MENU_BUILD_INDEX);
            #endif
        }
    }
    void AddProgress() {
        UserProgressController.Instance.ProgressState.Money += viewModel.CoinReward;
        UserProgressController.Instance.ProgressState.Spins += viewModel.DiamondReward;
    }
    public void AdRewardCoins(int multiplier) {
        canLoadNextLevel = false;
        HtmlBridge.Instance.ReportMetric(MetricNames.RewardWinCoin);
        HtmlBridge.Instance.ShowRewarded(() => {
            viewModel.CoinReward *= multiplier;
            SetCoins();
            canLoadNextLevel = true;
        }, () => {
            canLoadNextLevel = true;
        });
    }
    void AuthStatusRecieved(object arg) {
        LoadMenuLevel();
    }
    void Start() {
        EventManager.StartListening(EventNames.AuthStatusRecieved, AuthStatusRecieved);
    }
    void OnDestroy() {
        EventManager.StopListening(EventNames.AuthStatusRecieved, AuthStatusRecieved);
    }
}

public class ResultUIViewModel {
    public int CoinReward;
    public int DiamondReward;
    public bool ImposterDetected;
}