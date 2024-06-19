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
        { LocalizationKeys.AdSpinReady, "Готово" },
        { LocalizationKeys.HatsProgress, "Шляпы: {0}%" },
        { LocalizationKeys.BackpackProgress, "Рюкзаки: {0}%" },
        { LocalizationKeys.PerkNameExtraDamage, "Доп Урон" },
        { LocalizationKeys.PerkNameExtraHealth, "Доп ХП" },
        { LocalizationKeys.PerkNameWalkSpeed, "Медленная ходьба" },
        { LocalizationKeys.PerkNameExtraGuys, "Доп Амогус" },
        { LocalizationKeys.PerkNameFreezeBoss, "Стоп Боссы" },
        { LocalizationKeys.PerkDescriptionExtraDamage, "Увеличивает Урон на 50%" },
        { LocalizationKeys.PerkDescriptionExtraHealth, "Увеличивает ХП на 50%" },
        { LocalizationKeys.PerkDescriptionWalkSpeed, "Команда идёт медленнее" },
        { LocalizationKeys.PerkDescriptionExtraGuys, "Доп Амогус" },
        { LocalizationKeys.PerkDescriptionFreezeBoss, "Боссы стоят на месте" },
    };
    public Dictionary<string, string> LocalizedStringsEn = new Dictionary<string, string>() {
        { LocalizationKeys.YourRecord, "Your record" },
        { LocalizationKeys.AdSpinReady, "Ready" },
        { LocalizationKeys.HatsProgress, "Hats: {0}%" },
        { LocalizationKeys.BackpackProgress, "Backpacks: {0}%" },
        { LocalizationKeys.PerkNameExtraDamage, "Extra Damage" },
        { LocalizationKeys.PerkNameExtraHealth, "Extra Health" },
        { LocalizationKeys.PerkNameWalkSpeed, "Walk Speed" },
        { LocalizationKeys.PerkNameExtraGuys, "Extra Amogus" },
        { LocalizationKeys.PerkNameFreezeBoss, "Boss Stop" },
        { LocalizationKeys.PerkDescriptionExtraDamage, "Increase Damage by 50%" },
        { LocalizationKeys.PerkDescriptionExtraHealth, "Increase Health by 50%" },
        { LocalizationKeys.PerkDescriptionWalkSpeed, "Slow Walk Speed" },
        { LocalizationKeys.PerkDescriptionExtraGuys, "Extra Amogus on Start" },
        { LocalizationKeys.PerkDescriptionFreezeBoss, "Bosses do not Move" },
    };
    public static string GetPerkNameKey(PerkType perkType) {
        switch (perkType) {
            case PerkType.ExtraDamage: return LocalizationKeys.PerkNameExtraDamage;
            case PerkType.ExtraHealth: return LocalizationKeys.PerkNameExtraHealth;
            case PerkType.ExtraGuys: return LocalizationKeys.PerkNameExtraGuys;
            case PerkType.FreezeBoss: return LocalizationKeys.PerkNameFreezeBoss;
            case PerkType.WalkSpeed: return LocalizationKeys.PerkNameWalkSpeed;
            default: return "Unknown Perk Type";
        }
    }
    public static string GetPerkDescriptionKey(PerkType perkType) {
        switch (perkType) {
            case PerkType.ExtraDamage: return LocalizationKeys.PerkDescriptionExtraDamage;
            case PerkType.ExtraHealth: return LocalizationKeys.PerkDescriptionExtraHealth;
            case PerkType.ExtraGuys: return LocalizationKeys.PerkDescriptionExtraGuys;
            case PerkType.FreezeBoss: return LocalizationKeys.PerkDescriptionFreezeBoss;
            case PerkType.WalkSpeed: return LocalizationKeys.PerkDescriptionWalkSpeed;
            default: return "Unknown Perk Type";
        }
    }
}
public static class LocalizationKeys {
    public static string YourRecord = "YourRecord";
    public static string AdSpinReady = "AdSpinReady";
    public static string HatsProgress = "HatsProgress";
    public static string BackpackProgress = "BackpackProgress";
    public static string PerkNameExtraDamage = "PerkNameExtraDamage";
    public static string PerkNameExtraHealth = "PerkNameExtraHealth";
    public static string PerkNameWalkSpeed = "PerkNameWalkSpeed";
    public static string PerkNameExtraGuys = "PerkNameExtraGuys";
    public static string PerkNameFreezeBoss = "PerkNameFreezeBoss";
    public static string PerkDescriptionExtraDamage = "PerkDescriptionExtraDamage";
    public static string PerkDescriptionExtraHealth = "PerkDescriptionExtraHealth";
    public static string PerkDescriptionWalkSpeed = "PerkDescriptionWalkSpeed";
    public static string PerkDescriptionExtraGuys = "PerkDescriptionExtraGuys";
    public static string PerkDescriptionFreezeBoss = "PerkDescriptionFreezeBoss";
}
