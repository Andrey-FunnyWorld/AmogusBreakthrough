using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGate : MonoBehaviour {
    public Animator MyAnimator;
    public void Open() {
        MyAnimator.SetTrigger("open");
    }
}
