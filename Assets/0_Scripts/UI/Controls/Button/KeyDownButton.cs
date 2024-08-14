using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyDownButton : Button {
    public bool HandleDownKey = true;
    public override void OnPointerClick(PointerEventData eventData) {
        if (!HandleDownKey)
            base.OnPointerClick(eventData);
    }
    public override void OnPointerDown(PointerEventData eventData) {
        if (HandleDownKey)
            onClick.Invoke();
    }
}