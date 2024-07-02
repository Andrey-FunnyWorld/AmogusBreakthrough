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

        { LocalizationKeys.PerkNameAttackZoneVisibility, "Зона атаки" },
        { LocalizationKeys.PerkNameBossDamage, "Урон по боссам" },
        { LocalizationKeys.PerkNameBubble, "Щит" },
        { LocalizationKeys.PerkNameExtraAttackWidth, "Ширина атаки" },
        { LocalizationKeys.PerkNameExtraCoins, "Доп монеты" },
        { LocalizationKeys.PerkNameExtraGuy, "+1 в команду" },
        { LocalizationKeys.PerkNameExtraHealth, "Доп ХП" },
        { LocalizationKeys.PerkNameExtraHealthUltra, "Много ХП" },
        { LocalizationKeys.PerkNameOnePunchKill, "Зачистка" },
        { LocalizationKeys.PerkNameRegenHP, "Реген ХП" },
        { LocalizationKeys.PerkNameSlowWalkSpeed, "Медленнее" },
        { LocalizationKeys.PerkNameWeaponBoxTransparency, "Оружие в ящике" },

        { LocalizationKeys.PerkDescriptionAttackZoneVisibility, "Вы видите зону атаки" },
        { LocalizationKeys.PerkDescriptionBossDamage, "Доп урон по боссам" },
        { LocalizationKeys.PerkDescriptionBubble, "Временно защищает вас от урона" },
        { LocalizationKeys.PerkDescriptionExtraAttackWidth, "Увеличенная ширина атаки" },
        { LocalizationKeys.PerkDescriptionExtraCoins, "Доп монеты за прохождение" },
        { LocalizationKeys.PerkDescriptionExtraGuy, "+1 к начальном составу команды" },
        { LocalizationKeys.PerkDescriptionExtraHealth, "Доп здоровье" },
        { LocalizationKeys.PerkDescriptionExtraHealthUltra, "Много доп здоровья" },
        { LocalizationKeys.PerkDescriptionOnePunchKill, "Уничтожь всех врагов 1 раз" },
        { LocalizationKeys.PerkDescriptionRegenHP, "Здоровье восстанавливается" },
        { LocalizationKeys.PerkDescriptionSlowWalkSpeed, "Враги медленнее наступают" },
        { LocalizationKeys.PerkDescriptionWeaponBoxTransparency, "Заранее видно оружие в ящиках" },
    };
    public Dictionary<string, string> LocalizedStringsEn = new Dictionary<string, string>() {
        { LocalizationKeys.YourRecord, "Your record" },
        { LocalizationKeys.AdSpinReady, "Ready" },
        { LocalizationKeys.HatsProgress, "Hats: {0}%" },
        { LocalizationKeys.BackpackProgress, "Backpacks: {0}%" },
        
        { LocalizationKeys.PerkNameAttackZoneVisibility, "Attack Zone" },
        { LocalizationKeys.PerkNameBossDamage, "Boss Damage" },
        { LocalizationKeys.PerkNameBubble, "Shield" },
        { LocalizationKeys.PerkNameExtraAttackWidth, "Attack Width" },
        { LocalizationKeys.PerkNameExtraCoins, "Extra Coins" },
        { LocalizationKeys.PerkNameExtraGuy, "+1 teammate" },
        { LocalizationKeys.PerkNameExtraHealth, "Extra HP" },
        { LocalizationKeys.PerkNameExtraHealthUltra, "Extra Mega HP" },
        { LocalizationKeys.PerkNameOnePunchKill, "Wipe Enemies" },
        { LocalizationKeys.PerkNameRegenHP, "HP Regen" },
        { LocalizationKeys.PerkNameSlowWalkSpeed, "Slow Walk" },
        { LocalizationKeys.PerkNameWeaponBoxTransparency, "Weapon Box" },

        { LocalizationKeys.PerkDescriptionAttackZoneVisibility, "See attack zone" },
        { LocalizationKeys.PerkDescriptionBossDamage, "Cause more damage to bosses" },
        { LocalizationKeys.PerkDescriptionBubble, "Activate one-time shield" },
        { LocalizationKeys.PerkDescriptionExtraAttackWidth, "Extra attack width" },
        { LocalizationKeys.PerkDescriptionExtraCoins, "Extra coins on this level" },
        { LocalizationKeys.PerkDescriptionExtraGuy, "Extra teammate on startup" },
        { LocalizationKeys.PerkDescriptionExtraHealth, "Start with extra health" },
        { LocalizationKeys.PerkDescriptionExtraHealthUltra, "Start with mega health" },
        { LocalizationKeys.PerkDescriptionOnePunchKill, "Wipe out all the enemies" },
        { LocalizationKeys.PerkDescriptionRegenHP, "Regenerate health over time" },
        { LocalizationKeys.PerkDescriptionSlowWalkSpeed, "Enemies move slower" },
        { LocalizationKeys.PerkDescriptionWeaponBoxTransparency, "See weapons inside boxes" },
    };
    public static string GetPerkNameKey(PerkType perkType) {
        switch (perkType) {
            case PerkType.AttackZoneVisibility: return LocalizationKeys.PerkNameAttackZoneVisibility;
            case PerkType.BossDamage: return LocalizationKeys.PerkNameBossDamage;
            case PerkType.Bubble: return LocalizationKeys.PerkNameBubble;
            case PerkType.ExtraAttackWidth: return LocalizationKeys.PerkNameExtraAttackWidth;
            case PerkType.ExtraCoins: return LocalizationKeys.PerkNameExtraCoins;
            case PerkType.ExtraGuy: return LocalizationKeys.PerkNameExtraGuy;
            case PerkType.ExtraHealth: return LocalizationKeys.PerkNameExtraHealth;
            case PerkType.ExtraHealthUltra: return LocalizationKeys.PerkNameExtraHealthUltra;
            case PerkType.OnePunchKill: return LocalizationKeys.PerkNameOnePunchKill;
            case PerkType.RegenHP: return LocalizationKeys.PerkNameRegenHP;
            case PerkType.SlowWalkSpeed: return LocalizationKeys.PerkNameSlowWalkSpeed;
            case PerkType.WeaponBoxTransparency: return LocalizationKeys.PerkNameWeaponBoxTransparency;
            default: return "Unknown Perk Type";
        }
    }
    public static string GetPerkDescriptionKey(PerkType perkType) {
        switch (perkType) {
            case PerkType.AttackZoneVisibility: return LocalizationKeys.PerkDescriptionAttackZoneVisibility;
            case PerkType.BossDamage: return LocalizationKeys.PerkDescriptionBossDamage;
            case PerkType.Bubble: return LocalizationKeys.PerkDescriptionBubble;
            case PerkType.ExtraAttackWidth: return LocalizationKeys.PerkDescriptionExtraAttackWidth;
            case PerkType.ExtraCoins: return LocalizationKeys.PerkDescriptionExtraCoins;
            case PerkType.ExtraGuy: return LocalizationKeys.PerkDescriptionExtraGuy;
            case PerkType.ExtraHealth: return LocalizationKeys.PerkDescriptionExtraHealth;
            case PerkType.ExtraHealthUltra: return LocalizationKeys.PerkDescriptionExtraHealthUltra;
            case PerkType.OnePunchKill: return LocalizationKeys.PerkDescriptionOnePunchKill;
            case PerkType.RegenHP: return LocalizationKeys.PerkDescriptionRegenHP;
            case PerkType.SlowWalkSpeed: return LocalizationKeys.PerkDescriptionSlowWalkSpeed;
            case PerkType.WeaponBoxTransparency: return LocalizationKeys.PerkDescriptionWeaponBoxTransparency;
            default: return "Unknown Perk Type";
        }
    }
}
public static class LocalizationKeys {
    public static string YourRecord = "YourRecord";
    public static string AdSpinReady = "AdSpinReady";
    public static string HatsProgress = "HatsProgress";
    public static string BackpackProgress = "BackpackProgress";

    public static string PerkNameExtraGuy = "PerkNameExtraGuy";
    public static string PerkNameBossDamage = "PerkNameBossDamage";
    public static string PerkNameExtraHealth = "PerkNameExtraHealth";
    public static string PerkNameExtraAttackWidth = "PerkNameExtraAttackWidth";
    public static string PerkNameSlowWalkSpeed = "PerkNameSlowWalkSpeed";
    public static string PerkNameAttackZoneVisibility = "PerkNameAttackZoneVisibility";
    public static string PerkNameRegenHP = "PerkNameRegenHP";
    public static string PerkNameWeaponBoxTransparency = "PerkNameWeaponBoxTransparency";
    public static string PerkNameOnePunchKill = "PerkNameOnePunchKill";
    public static string PerkNameBubble = "PerkNameBubble";
    public static string PerkNameExtraCoins = "PerkNameExtraCoins";
    public static string PerkNameExtraHealthUltra = "PerkNameExtraHealthUltra";

    public static string PerkDescriptionExtraGuy = "PerkDescriptionExtraGuy";
    public static string PerkDescriptionBossDamage = "PerkDescriptionBossDamage";
    public static string PerkDescriptionExtraHealth = "PerkDescriptionExtraHealth";
    public static string PerkDescriptionExtraAttackWidth = "PerkDescriptionExtraAttackWidth";
    public static string PerkDescriptionSlowWalkSpeed = "PerkDescriptionSlowWalkSpeed";
    public static string PerkDescriptionAttackZoneVisibility = "PerkDescriptionAttackZoneVisibility";
    public static string PerkDescriptionRegenHP = "PerkDescriptionRegenHP";
    public static string PerkDescriptionWeaponBoxTransparency = "PerkDescriptionWeaponBoxTransparency";
    public static string PerkDescriptionOnePunchKill = "PerkDescriptionOnePunchKill";
    public static string PerkDescriptionBubble = "PerkDescriptionBubble";
    public static string PerkDescriptionExtraCoins = "PerkDescriptionExtraCoins";
    public static string PerkDescriptionExtraHealthUltra = "PerkDescriptionExtraHealthUltra";
}
