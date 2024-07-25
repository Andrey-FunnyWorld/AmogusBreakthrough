using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Cage : Attackable {
    public float BlinkDuration = 2;
    public MeshCololizer MeshCololizer;
    const float DESTROY_ANIMATION_LENGTH = 1;
    public override void Destroyed() {
        // add Amogus to the team
        base.Destroyed();
        EventManager.TriggerEvent(EventNames.CageDestroyed, this);
        StartCoroutine(Utils.WaitAndDo(DESTROY_ANIMATION_LENGTH, () => {
            StopCoroutine(blinkAnimation);
        }));
    }
    void Start() {
        blinkAnimation = StartCoroutine(Blink(BlinkDuration));
    }
    Coroutine blinkAnimation;
    IEnumerator Blink(float time) {
        float timer = 0;
        while (timer < time) {
            timer += Time.deltaTime;
            float alpha = -Math.Abs((timer - 1) / time) + 1;
            MeshCololizer.ChangeColor(new Color(MeshCololizer.Color.r, MeshCololizer.Color.g, MeshCololizer.Color.b, alpha));
            yield return null;
        }
        blinkAnimation = StartCoroutine(Blink(BlinkDuration));
    }
}
