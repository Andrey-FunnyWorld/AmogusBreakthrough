using System;
using System.Collections;
using UnityEngine;

public class Cage : Attackable {
    public float BlinkDuration = 2;
    public MeshCololizer MeshCololizer;
    public Transform ChainedAmogus;
    public Transform Icon;
    const float DESTROY_ANIMATION_LENGTH = 1;
    const float ICON_ANIMATION_DURATION = 1;
    const float ICON_ELEVATE_DISTANCE = 15;
    Coroutine blinkAnimation;

    public override void Destroyed() {
        base.Destroyed();
        EventManager.TriggerEvent(EventNames.CageDestroyed, this);
        StartCoroutine(Utils.WaitAndDo(DESTROY_ANIMATION_LENGTH, () => {
            StopCoroutine(blinkAnimation);
        }));
        StartCoroutine(AnimateIcon(ICON_ANIMATION_DURATION));
        Destroy(ChainedAmogus.gameObject);
    }

    protected override void Init() {
        base.Init();
        blinkAnimation = StartCoroutine(Blink(BlinkDuration));
    }

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
    IEnumerator AnimateIcon(float time) {
        float timer = 0;
        float startY = Icon.transform.position.y;
        while (timer < time) {
            timer += Time.deltaTime;
            float delta = ICON_ELEVATE_DISTANCE * timer / time;
            Icon.transform.position = new Vector3(Icon.transform.position.x, startY + delta, Icon.transform.position.z);
            yield return null;
        }
        Destroy(Icon.gameObject);
    }
}
