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
    // UI ->
    public static string RandomSkins = "RandomSkins";
    public static string WheelSpinResult = "WheelSpinResult";
    public static string WheelSpinStart = "WheelSpinStart";
    public static string SkinItemEquip = "SkinItemEquip";
    public static string ShopItemPurchaseTry = "ShopItemPurchaseTry";
    public static string ShopItemPurchased = "ShopItemPurchased";
    public static string FreeSpinChanged = "FreeSpinChanged";
    public static string PerkSelected = "PerkSelected";
    public static string WeaponChanged = "WeaponChanged";
}
