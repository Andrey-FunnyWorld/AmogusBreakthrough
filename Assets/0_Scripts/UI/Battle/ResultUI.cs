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
    public void LoadMenuLevel() {
        LevelLoader.LoadScene(LevelLoader.MENU_BUILD_INDEX);
    }
}

public class ResultUIViewModel {
    public int CoinReward;
    public int DiamondReward;
    public bool ImposterDetected;
}