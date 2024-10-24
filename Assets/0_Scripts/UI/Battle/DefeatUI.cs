using UnityEngine;

public class DefeatViewModel {
    public int CoinReward;
}

public class DefeatUI : MonoBehaviour {
    public ScoreText CoinText, AdCoinText;
    public LevelLoader LevelLoader;
    public Transform AdButton;
    bool canLoadNextLevel = true;
    DefeatViewModel viewModel;
    public AudioRandomShot Audio;
    public int CoinsAdReward = 70;

    void Start() {
        AdCoinText.Score = CoinsAdReward;
    }

    public void AddRewardedCoins(int extraCoins) {
        HtmlBridge.Instance.ReportMetric(MetricNames.RewardDefeatCoin);
        viewModel.CoinReward += extraCoins;
        SetCoins();
    }
    // public void AdRewardCoins() {
    //     canLoadNextLevel = false;
    //     HtmlBridge.Instance.ReportMetric(MetricNames.RewardDefeatCoin);
    //     HtmlBridge.Instance.ShowRewarded(() => {
    //         viewModel.CoinReward += CoinsAdReward;
    //         SetCoins();
    //         canLoadNextLevel = true;
    //     }, () => {
    //         canLoadNextLevel = true;
    //     });
    // }

    public void ShowResult(DefeatViewModel vm) {
        viewModel = vm;
        gameObject.SetActive(true);
        AdButton.gameObject.SetActive(UserProgressController.Instance.ProgressState.CompletedRoundsCount > 0);
        Audio.Play();
        SetCoins();
        HtmlBridge.Instance.ReportMetric(MetricNames.Lose);
        LoserAssistant.LastRoundIsLost = true;
        LoserAssistant.LostRounds++;
        LoserAssistant.RoundsPlayed++;
    }

    public void SetCoins() {
        CoinText.Score = viewModel.CoinReward;
    }

    public void FinishLevel() {
        if (canLoadNextLevel) {
            if (UserProgressController.Instance.ProgressState.CompletedRoundsCount > 0) {
                UserProgressController.Instance.ProgressState.Money += viewModel.CoinReward;
                UserProgressController.Instance.SaveProgress();
            }
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
