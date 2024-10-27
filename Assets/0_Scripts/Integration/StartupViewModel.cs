using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StartupViewModel {
    public string Locale;
    public string Platform;
    public string IsLogged;
    public PlayerSettings Settings;
    public ProgressState Progress;
}

public enum PlatformType {
    Desktop, Android, IOS
}

public static class MetricNames {
    public static string BattleLevelStarted = "BattleLevelStarted";
    public static string ImposterDetected = "ImposterDetected";
    public static string ImposterFailed = "ImposterFailed";
    public static string RewardWheel = "RewardWheel";
    public static string RewardWinCoin = "RewardWinCoin";
    public static string RewardDefeatCoin = "RewardDefeatCoin";
    public static string DifficultyNoob = "DifficultyNoob";
    public static string DifficultyPro = "DifficultyPro";
    public static string DifficultyHacker = "DifficultyHacker";
    public static string Win = "RobbyAmogusWin";
    public static string Lose = "RobbyAmogusLose";
    public static string SkinPurchased = "RobbyAmogusSkinPurchased";
    public static string PerkPurchased = "RobbyAmogusPerkPurchased";
    public static string UpgradePurchased = "RobbyAmogusUpgradePurchased";
    public static string GiftAd = "RobbyAmogusGiftAd";
    // track new players
    public static string FirstGameReady = "FirstGameReady";
    public static string FirstBattleStarted = "FirstBattleStarted";
    public static string FirstImposterStarted = "FirstImposterStarted";
    public static string FirstImposterSelected = "FirstImposterSelected";
    public static string FirstTutorialComplete = "FirstTutorialComplete";
}