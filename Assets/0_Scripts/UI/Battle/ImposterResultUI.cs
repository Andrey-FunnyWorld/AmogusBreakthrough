using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImposterResultUI : MonoBehaviour {
    public Animator Animator;
    public UnityEvent AnimationEnded;
    public Transform Success, Fail;
    public void ShowResult(bool success) {
        (success ? Success :  Fail).gameObject.SetActive(true);
        Animator.SetTrigger(success ? "like" : "dislike");
    }
    public void AnimationFinished() {
        AnimationEnded.Invoke();
    }
}
