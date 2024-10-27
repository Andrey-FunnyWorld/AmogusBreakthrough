using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyDownButton : MonoBehaviour, IPointerDownHandler {
    public KeyCode DownKey = KeyCode.Space;
    public UnityEvent Down;
    public void OnPointerDown(PointerEventData eventData) {
        Down.Invoke();
    }
    void Update() {
        if (Input.GetKeyDown(DownKey)) {
            Down.Invoke();
        }
    }
}