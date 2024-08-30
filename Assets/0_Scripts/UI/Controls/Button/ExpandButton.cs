using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandButton : MonoBehaviour {
    public float RollingTime = 0.3f;
    public RectTransform ContentToRoll, MaskRect, Icon;
    public ButtonHint[] ButtonHints;
    public float TimeForHints = 4;
    bool isOpened = false;
    Coroutine waitHintsCoroutine, rollCoroutine;
    public void SetOpened(bool open) {
        if (rollCoroutine != null) {
            StopCoroutine(rollCoroutine);
            Icon.localRotation = Quaternion.identity;
        }
        isOpened = open;
        if (waitHintsCoroutine != null)
            StopCoroutine(waitHintsCoroutine);
        if (open) {
            rollCoroutine = StartCoroutine(Roll());
            waitHintsCoroutine = StartCoroutine(Utils.WaitAndDo(TimeForHints, () => SetHintsVisibility(true)));
        } else {
            SetHintsVisibility(false);
            ContentToRoll.anchoredPosition = new Vector2(0, -MaskRect.sizeDelta.y);
        }
    }
    IEnumerator Roll() {
        float timer = 0;
        while (timer < RollingTime) {
            timer += Time.deltaTime;
            float part = Utils.EaseInCubic(1 - timer / RollingTime);
            float y = MaskRect.sizeDelta.y * part;
            ContentToRoll.anchoredPosition = new Vector2(0, -y);
            Icon.localRotation = Quaternion.Euler(0, 0, part * 360);
            yield return null;
        }
        Icon.localRotation = Quaternion.identity;
    }
    void Start() {
        SetOpened(false);
    }
    public void ToggleButton() {
        SetOpened(!isOpened);
    }
    public bool IsOpened { get { return isOpened; }}
    public void SetHintsVisibility(bool visible) {
        foreach (ButtonHint hint in ButtonHints)
            hint.gameObject.SetActive(visible);
    }
}
