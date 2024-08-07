using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class HtmlBridge : MonoBehaviour, IPlatformBridge {
    public static string TRUE = "true";
    public static bool AdsIsVisible = false;
    public static PlatformType PlatformType;
    public static bool IsLogged = false;
    UnityAction rewardedSuccessCallback, rewardedFailCallback;
    UnityAction interstitialClosedCallback;
    bool rewardSuccess = false;
    bool boot = true;
    bool readyToPlayPinged = false;

    #region Load and Save
    PlatformType GetPlatformTypeByString(string platformTypeStr) {
        switch (platformTypeStr) {
            case "android": return PlatformType.Android;
            case "ios": return PlatformType.IOS;
            default: return PlatformType.Desktop;
        }
    }
    public void ReceiveStartupData(string startupData) {
        StartupViewModel vm = startupData != string.Empty ? JsonUtility.FromJson<StartupViewModel>(startupData) : new StartupViewModel();
        PlatformType = GetPlatformTypeByString(vm.Platform);
        IsLogged = vm.IsLogged == TRUE;
        MyLocalization.Instance.CurrentLanguage = vm.Locale;
        if (IsLogged)
            UserProgressController.Instance.ProgressState.SkipSaveTargetDialog = true;
        UserProgressController.Instance.ProgressState = vm.Progress;
        UserProgressController.Instance.PlayerSettings = vm.Settings;
        if (boot) {
            StartCoroutine(Utils.WaitAndDo(0.1f, () => {
                UserProgressController.ProgressLoaded = true;
                EventManager.TriggerEvent(EventNames.StartDataLoaded, vm);
            }));
        }
        boot = false;
    }
    public void AskToLogin() {
        AskToLoginExtern();
    }
    public void ReceiveAuthRequestResultString(string isLogged) {
        ReceiveAuthRequestResult(isLogged == TRUE);
    }
    public void ReceiveAuthRequestResult(bool isLogged) { // если у пользователя уже были сохранения, то перед этим методом вызовется ReceiveStartupData
        IsLogged = isLogged;
        UserProgressController.Instance.ProgressState.SkipSaveTargetDialog = true;
        SaveProgress();
        EventManager.TriggerEvent(EventNames.AuthStatusRecieved, isLogged);
    }
    public void SaveProgress() {
        #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN
        ProgressFileSaver.SaveProgress(UserProgressController.Instance.ProgressState);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        string json = JsonUtility.ToJson(UserProgressController.Instance.ProgressState);
        SaveExtern(json);
        #endif
    }
    public void SaveSettings() {
        #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN
        ProgressFileSaver.SaveSettings(UserProgressController.Instance.PlayerSettings);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        string json = JsonUtility.ToJson(UserProgressController.Instance.PlayerSettings);
        SaveExtern(json);
        #endif
    }
    #endregion

    #region ads
    public void ShowRewarded(UnityAction successCallback, UnityAction failCallback) {
        if (!AdsIsVisible) {
            rewardSuccess = false;
            //if (PlatformType == PlatformType.Desktop) Cursor.lockState = CursorLockMode.None;
            AdsIsVisible = true;
            AudioListener.volume = 0;
            rewardedSuccessCallback = successCallback;
            rewardedFailCallback = failCallback;
            ShowRewardedExtern();
        }
    }
    public void ReceiveRewardedResultString(string success) {
        ReceiveRewardedResult(success == TRUE);
    }
    public void ReceiveRewardedResult(bool success) {
        if (success)
            rewardSuccess = true;
        else {
            AudioListener.volume = 1;
            //if (PlatformType == PlatformType.Desktop) Cursor.lockState = CursorLockMode.Locked;
            rewardedFailCallback.Invoke();
        }
    }
    public void RewardedClosed() {
        AudioListener.volume = 1;
        AdsIsVisible = false;
        //if (PlatformType == PlatformType.Desktop) Cursor.lockState = CursorLockMode.Locked;
        (rewardSuccess ? rewardedSuccessCallback : rewardedFailCallback).Invoke();
    }
    public void ShowInterstitial(UnityAction closedCallback) {
        if (!AdsIsVisible) {
            #if UNITY_WEBGL && !UNITY_EDITOR
            AdsIsVisible = true;
            interstitialClosedCallback = closedCallback;
            AudioListener.volume = 0;
            ShowInterstitialExtern();
            #endif
        }
    }
    public void InterstitialClosed() {
        AudioListener.volume = 1;
        AdsIsVisible = false;
        if (interstitialClosedCallback != null)
            interstitialClosedCallback.Invoke();
    }
    #endregion

    public void ReadyToPlay() {
        if (!readyToPlayPinged) {
            readyToPlayPinged = true;
            #if UNITY_WEBGL && !UNITY_EDITOR
            PingYandexReadyExtern();
            #endif
        }
    }
    public void RateGame() {
        RateGameExtern();
    }
    public void ReportMetric(string id) {
        #if UNITY_WEBGL && !UNITY_EDITOR
        ReportMetricExtern(id);
        #endif
    }
    [DllImport("__Internal")]
    private static extern void SaveExtern(string data);
    [DllImport("__Internal")]
    private static extern void AskToLoginExtern();
    [DllImport("__Internal")]
    private static extern void ShowRewardedExtern();
    [DllImport("__Internal")]
    private static extern void ShowInterstitialExtern();
    [DllImport("__Internal")]
    private static extern void PingYandexReadyExtern();
    [DllImport("__Internal")]
    private static extern void ReportMetricExtern(string id);
    [DllImport("__Internal")]
    private static extern void RateGameExtern();

    public static HtmlBridge Instance;
    void Awake() {
        if (Instance == null) {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
}
