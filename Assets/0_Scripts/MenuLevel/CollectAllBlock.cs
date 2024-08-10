using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectAllBlock : MonoBehaviour {
    public Animator Animator;
    public float ShowDelay = 2;
    public int ShowAnimationCount = 4;
    public void Show() {
        float time = Random.Range(ShowDelay - 0.1f, ShowDelay + 0.1f);
        StartCoroutine(Utils.WaitAndDo(time, () => { 
            int index = Random.Range(0, ShowAnimationCount);
            Animator.SetInteger("show", index);
        }));
    }
}
