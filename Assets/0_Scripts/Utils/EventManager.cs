using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, MyUnityEvent> eventDictionary;
    private static EventManager eventManager;

    public static EventManager instance
    {
        get {
            if (!eventManager) {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!eventManager) {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                } else {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init() {
        if (eventDictionary == null) {
            eventDictionary = new Dictionary<string, MyUnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction<object> listener) {
        MyUnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.AddListener(listener);
        } else {
            thisEvent = new MyUnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<object> listener) {
        if (eventManager == null) return;
        MyUnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, object param = null) {
        MyUnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.Invoke(param);
        }
    }
}

public class MyUnityEvent : UnityEvent<object> {

}

public static class EventNames {
    public static string StartMovement = "StartMovement";
    public static string CageDestroyed = "CageDestroyed";
    public static string RoadFinished = "RoadFinished";
    public static string StartDataLoaded = "StartDataLoaded";
    public static string LevelLoaded = "LevelLoaded";
    public static string TeamDead = "TeamDead";

    public static string HpChanged = "HpChanged";
    public static string EnemyDied = "EnemyDied";

    public static string AbilityOnePunch = "AbilityOnePunch";
    public static string AbilityBubble = "AbilityBubble";

    public static string MatesChanged = "MatesChanged";
    public static string RequestSpawnWeapon = "RequestSpawnWeapon";
    // UI ->
    public static string RandomSkins = "RandomSkins";
    public static string WheelSpinResult = "WheelSpinResult";
    public static string WheelSpinStart = "WheelSpinStart";
    public static string SkinItemEquip = "SkinItemEquip";
    
    public static string FreeSpinChanged = "FreeSpinChanged";
    public static string PerkSelected = "PerkSelected";
    public static string WeaponChanged = "WeaponChanged";

    public static string ShopItemPurchaseTry = "ShopItemPurchaseTry";
    public static string ShopItemPurchased = "ShopItemPurchased";
    public static string PerkItemPurchaseTry = "PerkItemPurchaseTry";
    public static string PerkItemPurchased = "PerkItemPurchased";
    public static string UpgradeItemPurchaseTry = "UpgradeItemPurchaseTry";
    public static string UpgradeItemPurchased = "UpgradeItemPurchased";
    public static string SkipAdPurchased = "SkipAdPurchased";
    public static string NotEnoughMoney = "NotEnoughMoney";
}
