using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLocalization : MonoBehaviour {
    public string CurrentLanguage = "ru";
    public static MyLocalization Instance;
    void Awake() {
        if (Instance == null) {
            transform.parent = null;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    public string GetLocalizedText(string key) {
        return CurrentLanguage == "ru" ? LocalizedStringsRu[key] : LocalizedStringsEn[key];
    }
    public Dictionary<string, string> LocalizedStringsRu = new Dictionary<string, string>() {
        { LocalizationKeys.YourRecord, "Ваш рекорд" },
    };
    public Dictionary<string, string> LocalizedStringsEn = new Dictionary<string, string>() {
        { LocalizationKeys.YourRecord, "Your record" },
    };
}
public static class LocalizationKeys {
    public static string YourRecord = "YourRecord";
}
