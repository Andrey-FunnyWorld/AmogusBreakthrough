using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBox : MonoBehaviour {
    public Animator Animator;
    [NonSerialized]
    public GiftController GiftController;
    [HideInInspector]
    public bool CanClick { get; set; } = true;
    bool opened = false;
    void Click() {
        if (CanClick) {
            GiftController.ShowDialog();
        }
    }
    public void Open() {
        if (!opened) {
            opened = true;
            Animator.SetTrigger("open");
        }
    }
    void OnTouchDown() {
        Click();
    }
    void OnMouseDown() {
        Click();
    }
}
