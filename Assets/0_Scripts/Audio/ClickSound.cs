using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSound : MonoBehaviour, IPointerDownHandler {
    public UISoundType SoundType;
    public void OnPointerDown(PointerEventData e) {
        EventManager.TriggerEvent(EventNames.PlayUISound, SoundType);
    }
}
