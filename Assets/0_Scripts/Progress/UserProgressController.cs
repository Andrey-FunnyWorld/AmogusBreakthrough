using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserProgressController : MonoBehaviour {
    public static UserProgressController Instance;
    [System.NonSerialized]
    public ProgressState ProgressState;
    [System.NonSerialized]
    public PlayerSettings PlayerSettings;
    [NonSerialized]
    public bool IsLogged = false;
    public static bool ProgressLoaded = false;
    public static string TRUE = "true";
    public SkinItems BackpackItems;

    void Awake() {
        if (Instance == null) {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            Instance = this;
            #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN
            LoadFromFile();
            #endif
        } else {
            Destroy(gameObject);
        }
    }
    public void LoadFromFile() {
        PlayerSettings = ProgressFileSaver.LoadSettings();
        ProgressState = ProgressFileSaver.LoadProgress();
        ProgressState.SkipSaveTargetDialog = true;
        //ProgressState.AdjustHats(BackpackItems.Items.Length - 1); // 1 is for None
        //HtmlBridge.PlatformType = PlatformType.Android;
        StartCoroutine(Utils.WaitAndDo(0.1f, () => {
            ProgressLoaded = true;
            EventManager.TriggerEvent(EventNames.StartDataLoaded, null);
        }));
    }
    public void SaveProgress() {
        HtmlBridge.Instance.SaveProgress();
    }
    public void SaveSettings() {
        HtmlBridge.Instance.SaveSettings();
    }
}

[Serializable]
public class ProgressState {
    public int Money;
    public int Spins;
    public int[] EquippedBackpacks;
    public int[] EquippedHats;
    public int[] PurchasedBackpacks;
    public int[] PurchasedHats;
    public int[] PurchasedPerks;
    public int UpgradeLevelHP;
    public int UpgradeLevelAttackSpeed;
    public int UpgradeLevelDamage;
    public int SkipAdRounds = 0;
    public bool SkipSaveTargetDialog;
    public string AdSpinWhenAvailableString;
    public int CompletedRoundsCount = 0;
    public int ImposterDetectedCount = 0;
    public bool AskedForRating = false;
    public int TutorialStage = 0;
    public bool ShowMenuOnStart = false;
    public int LastGiftedMilestone;
    public DateTime AdSpinWhenAvailable {
        get {
            if (AdSpinWhenAvailableString != string.Empty)
                return DateTime.Parse(AdSpinWhenAvailableString);
            else return new DateTime(1900, 0, 0);
        }
    }
    public ProgressState() {
        DefaultValues(false);
    }
    void DefaultValues(bool debug = false) {
        AdSpinWhenAvailableString = DateTime.Now.ToString();
        PurchasedPerks = new int[4] { 0, 1, 2, 3 };
        PurchasedBackpacks = new int[1] { 0 };
        PurchasedHats = new int[1] { 0 };
        EquippedBackpacks = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // 0 - skin for robby. 1 - 10 skins for amoguses
        EquippedHats = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // 0 - hat for robby. 1 - 10 hats for amoguses
        LastGiftedMilestone = -1;
        if (debug) {
            EquippedBackpacks = new int[11] { 0, 0, 2, 3, 0, 0, 0, 0, 0, 0, 0 }; // 0 - skin for robby. 1 - 10 skins for amoguses
            EquippedHats = new int[11] { 0, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0 }; // 0 - hat for robby. 1 - 10 hats for amoguses
            SkipSaveTargetDialog = false;
            Spins = 2;
            Money = 900;
            AdSpinWhenAvailableString = DateTime.Now.AddSeconds(15).ToString();// "05/19/2024 19:41:35";
            PurchasedPerks = new int[4] { 0, 1, 2, 3 };
            UpgradeLevelDamage = 7;
        }
    }
    public void Equipp(SkinType skinType, SkinItemName skinName, int index) {
        int[] skinItemArray = skinType == SkinType.Hat ? EquippedHats : EquippedBackpacks;
        skinItemArray[index] = (int)skinName;
    }
    public void Equipp(SkinType skinType, SkinItemName skinName) {
        int[] skinItemArray = skinType == SkinType.Hat ? EquippedHats : EquippedBackpacks;
        for (int i = 0; i < skinItemArray.Length; i++) {
            skinItemArray[i] = (int)skinName;
        }
    }
    public void AddPurchased(SkinType skinType, SkinItemName skinName) {
        int[] skinItemArray = skinType == SkinType.Hat ? PurchasedHats : PurchasedBackpacks;
        int[] newItems = new int[skinItemArray.Length + 1];
        Array.Copy(skinItemArray, newItems, skinItemArray.Length);
        newItems[newItems.Length - 1] = (int)skinName;
        if (skinType == SkinType.Hat) {
            PurchasedHats = newItems;
        } else {
            PurchasedBackpacks = newItems;
        }
    }
    public void AddPurchased(PerkType perkType) {
        int[] newItems = new int[PurchasedPerks.Length + 1];
        Array.Copy(PurchasedPerks, newItems, PurchasedPerks.Length);
        newItems[newItems.Length - 1] = (int)perkType;
        PurchasedPerks = newItems;
    }
    public void AddUpgrade(UpgradeType upgradeType, int level) {
        switch (upgradeType) {
            case UpgradeType.AttackSpeed: UpgradeLevelAttackSpeed = level; break;
            case UpgradeType.Damage: UpgradeLevelDamage = level; break;
            case UpgradeType.HP: UpgradeLevelHP = level; break;
        }
    }
    public bool IsAllSkinsCollected() {
        return PurchasedHats.Length + PurchasedBackpacks.Length >= Enum.GetValues(typeof(SkinItemName)).Length + 1;
    }
    // public void AdjustHats(int startIndex) {
    //     for (int i = 0; i < EquippedHats.Length; i++) {
    //         if (EquippedHats[i] > 0)
    //             EquippedHats[i] += startIndex;
    //     }
    //     for (int i = 0; i < PurchasedHats.Length; i++) {
    //         if (PurchasedHats[i] > 0)
    //             PurchasedHats[i] += startIndex;
    //     }
    // }
}

[Serializable]
public class PlayerSettings {
    public PlayerSettings() {
        MusicVolume = 0.5f;
        SoundVolume = 0.5f;
    }
    public float MusicVolume;
    public float SoundVolume;
}
