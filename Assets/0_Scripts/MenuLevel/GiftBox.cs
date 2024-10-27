using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBox : MonoBehaviour {
    public Animator Animator;
    [NonSerialized]
    public GiftController GiftController;
    bool opened = false;
    void Click() {
        GiftController.ShowDialog();
            // Animator.SetTrigger("open");
            // GiftController.GetGift(() => {
            //     Destroy(gameObject);
            // });
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
