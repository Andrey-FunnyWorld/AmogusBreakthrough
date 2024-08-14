using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldDownButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [HideInInspector]
    public bool IsDown;
    public void OnPointerDown(PointerEventData eventData) {
        IsDown = true;
    }
    public void OnPointerUp(PointerEventData eventData) {
        IsDown = false;
    }
}
