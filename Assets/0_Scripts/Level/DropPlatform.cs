using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPlatform : MonoBehaviour {
    public Animator Animator;
    public void Drop() {
        Animator.SetBool("drop", true);
    }
}
