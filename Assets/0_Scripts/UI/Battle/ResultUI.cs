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
    public MyDialog LoginDialog;
    ResultUIViewModel viewModel;
    public void ShowImposterResult(bool success) {
        ImposterResultUI.ShowResult(success);
    }
    public void ShowResult(ResultUIViewModel vm) {
        viewModel = vm;
        ResultPanel.gameObject.SetActive(true);
        ImposterDetected.gameObject.SetActive(false);
        ImposterNotDetected.gameObject.SetActive(false);
        Animator.SetTrigger("show");
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
        UserProgressController.Instance.SaveProgress();
        LevelLoader.LoadScene(LevelLoader.MENU_BUILD_INDEX);
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