using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPlatformBridge {
    void ReceiveStartupData(string startupData);
    void AskToLogin();
    void ReceiveAuthRequestResult(bool isLogged);
    void SaveProgress();
    void SaveSettings();
    
    void ShowRewarded(UnityAction successCallback, UnityAction failCallback);
    void ReceiveRewardedResult(bool success);
    void RewardedClosed();
    void ShowInterstitial(UnityAction closedCallback);
    void InterstitialClosed();

    void ReadyToPlay();
    void RateGame();

    void ReportMetric(string id);
    // leaderboards...
}
