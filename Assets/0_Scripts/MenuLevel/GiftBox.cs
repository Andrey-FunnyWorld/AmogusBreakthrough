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
        if (!opened) {
            opened = true;
            Animator.SetTrigger("open");
            GiftController.GetGift(() => {
                Destroy(gameObject);
            });
        }
    }
    void OnTouchDown() {
        Click();
    }
    void OnMouseDown() {
        Click();
    }
}
