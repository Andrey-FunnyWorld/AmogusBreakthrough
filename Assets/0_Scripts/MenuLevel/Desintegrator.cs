using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Desintegrator : MonoBehaviour {
    public Animator Animator;
    public float VFXDuration = 0.6f;
    public float MaxScale = 5f;
    public Transform VFXPrefab;
    [NonSerialized]
    public bool IsRunning = false;
    UnityAction finishAction;
    bool success = false;
    public void Test(bool success, UnityAction finishAction) {
        this.success = success;
        Animator.SetTrigger(success ? "success" : "fail");
        IsRunning = true;
        this.finishAction = finishAction;
    }
    public void FinishAttempt() {
        //IsRunning = false;
        finishAction.Invoke();
        if (success) {
            Success();
        }
    }
    public void Bang() {
        Wave(transform.position);
    }
    void Success() {
        
    }
    public void Wave(Vector3 pos) {
        Transform vfx = Instantiate(VFXPrefab, pos, Quaternion.identity);
        StartCoroutine(AnimateVFX(vfx));
    }
    IEnumerator AnimateVFX(Transform vfx) {
        float timer = 0;
        float scale = 1;
        while (timer < VFXDuration) {
            timer += Time.deltaTime;
            scale += timer / VFXDuration * MaxScale;
            vfx.localScale = new Vector3(scale, vfx.localScale.y, scale);
            yield return null;
        }
        Destroy(vfx.gameObject);
    }
}
