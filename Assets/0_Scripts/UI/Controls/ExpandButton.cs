using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandButton : MonoBehaviour {
    public float RollingTime = 0.3f;
    public RectTransform ContentToRoll, MaskRect, Icon;
    bool isOpened = false;
    public void SetOpened(bool open) {
        isOpened = open;
        if (open) {
            StartCoroutine(Roll());
        } else {
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
}
