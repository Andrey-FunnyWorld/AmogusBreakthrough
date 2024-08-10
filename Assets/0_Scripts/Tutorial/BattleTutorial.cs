using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTutorial : MonoBehaviour, IPlatformAdaptable {
    public Image Image;
    public Sprite Touch, Desktop;
    public float ShowDelay = 3;
    Coroutine moveCoroutine;
    public float MoveDistance = 200;
    public float Duration = 1;
    void OnEnable() {
        StartCoroutine(MoveImage(Duration));
    }
    void OnDisable() {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
    }
    IEnumerator MoveImage(float time) {
        float timer = 0;
        while (timer < time) {
            timer += Time.deltaTime;
            float ratio = -Mathf.Abs((timer / time) * 2 - 1) + 0.5f;
            Image.rectTransform.anchoredPosition = new Vector2(MoveDistance * ratio, Image.rectTransform.anchoredPosition.y);
            yield return null;
        }
        moveCoroutine = StartCoroutine(MoveImage(time));
    }
    public void Adapt(PlatformType platformType)  {
        Image.sprite = platformType == PlatformType.Desktop ? Desktop : Touch;
    }
}
