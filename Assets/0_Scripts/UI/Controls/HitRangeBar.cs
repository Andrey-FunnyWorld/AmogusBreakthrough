using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HitRangeBar : MonoBehaviour {
    public RectTransform Thumb, TargetArea, BarArea;
    public ButtonDisabled TestButton;
    public KeyCode TestKey = KeyCode.Space;
    public UnityEvent<bool> ClickAction;
    public float TargetRatio = 0.1f;
    public float ThumbSpeed = 3;
    float thumbPosition = 0;
    float minHit, maxHit;
    bool moveLeft = true;
    bool clicked = false;
    Coroutine thumbCoroutine;
    public void RunThumb() {
        clicked = false;
        thumbPosition = Random.Range(0f, 1f);
        if (thumbCoroutine != null)
            StopCoroutine(thumbCoroutine);
        thumbCoroutine = StartCoroutine(ThumbMoving());
    }
    public void SetParams(float thumbSpeed, float targetRatio) {
        if (thumbCoroutine != null)
            StopCoroutine(thumbCoroutine);
        ThumbSpeed = thumbSpeed;
        TargetRatio = targetRatio;
        minHit = .5f - TargetRatio / 2;
        maxHit = .5f + TargetRatio / 2;
        TargetArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BarArea.rect.width * TargetRatio);
    }
    IEnumerator ThumbMoving() {
        int sign = moveLeft ? 1 : -1;
        while (!clicked) {
            thumbPosition += ThumbSpeed * Time.deltaTime * sign;
            if (thumbPosition >= 1 || thumbPosition <= 0) {
                moveLeft = !moveLeft;
                sign = moveLeft ? 1 : -1;
                thumbPosition = Mathf.Clamp(thumbPosition, 0, 1);
            }
            MoveThumb(thumbPosition);
            yield return null;
        }
    }
    void MoveThumb(float pos) {
        Thumb.anchoredPosition = new Vector2(BarArea.rect.width * pos, 0);
    }
    public void Click() {
        if (!clicked) {
            clicked = true;
            ClickAction.Invoke(IsHit(thumbPosition));
        }
        //return IsHit(thumbPosition);
    }
    bool IsHit(float pos) {
        return pos >= minHit && pos <= maxHit;
    }
    void Update() {
        if (!clicked && Input.GetKeyDown(TestKey)) {
            Click();
        }
    }
}
