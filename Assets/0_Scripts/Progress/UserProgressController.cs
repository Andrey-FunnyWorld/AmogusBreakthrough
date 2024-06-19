using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class UserProgressController : MonoBehaviour {
    public static UserProgressController Instance;
    [System.NonSerialized]
    public ProgressState ProgressState;
    [System.NonSerialized]
    public PlayerSettings PlayerSettings;
    public bool IsLogged = false;
    public bool ProgressLoaded;
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
        ProgressState.AdjustHats(BackpackItems.Items.Length - 1); // 1 is for None
        StartCoroutine(Utils.WaitAndDo(0.1f, () => {
            Instance.ProgressLoaded = true;
            EventManager.TriggerEvent(EventNames.StartDataLoaded, null);
        }));
    }
}

[System.Serializable]
public class ProgressState {
    public int Money;
    public int Spins;
    public int[] EquippedBackpacks;
    public int[] EquippedHats;
    public int[] PurchasedBackpacks;
    public int[] PurchasedHats;
    [System.NonSerialized]
    public bool SkipSaveTargetDialog;
    public string AdSpinWhenAvailableString;
    public DateTime AdSpinWhenAvailable {
        get { 
            if (AdSpinWhenAvailableString != string.Empty)
                return DateTime.Parse(AdSpinWhenAvailableString);
            else return new DateTime(1900, 0, 0);
        }
    }
    public ProgressState() {
        DefaultValues();
    }
    void DefaultValues() {
        EquippedBackpacks = new int[11] { 0, 0, 2, 3, 0, 0, 0, 0, 0, 0, 0 }; // 0 - skin for robby. 1 - 10 skins for amoguses
        EquippedHats = new int[11] { 0, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0 }; // 0 - hat for robby. 1 - 10 hats for amoguses
        PurchasedBackpacks = new int[1] { 0 };
        PurchasedHats = new int[1] { 0 };
        SkipSaveTargetDialog = false;
        Spins = 2;
        AdSpinWhenAvailableString = DateTime.Now.AddSeconds(15).ToString();// "05/19/2024 19:41:35";
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
    public void AdjustHats(int startIndex) {
        for (int i = 0; i < EquippedHats.Length; i++) {
            if (EquippedHats[i] > 0)
                EquippedHats[i] += startIndex;
        }
        for (int i = 0; i < PurchasedHats.Length; i++) {
            if (PurchasedHats[i] > 0)
                PurchasedHats[i] += startIndex;
        }
    }
}
[System.Serializable]
public class PlayerSettings {
    public PlayerSettings() {
        MusicVolume = 0.5f;
        SoundVolume = 0.5f;
    }
    public float MusicVolume;
    public float SoundVolume;
}
