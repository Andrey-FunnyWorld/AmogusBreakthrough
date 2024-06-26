using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressBarUI : MonoBehaviour {
    public RectTransform barRectTransform, progressImage;
    public TextMeshProUGUI valueText;
    public bool EnableValueChangeAnimation = false;
    public bool EnableTextChangeAnimation = false;
    public bool HideWhenFull = true;

    protected float animationDuration = 1.0f;
    [HideInInspector]
    public bool isInitialized = false;
    public float Value {
        get { return fValue; }
        set {
            if (fValue != value) {
                prevValue = fValue;
                fValue = Mathf.Min(value, MaxValue);
                UpdateProgressCore();
            }
        }
    }
    public float MaxValue {
        get { return maxValue; }
        set {
            if (maxValue != value) {
                maxValue = value;
                UpdateProgressCore();
            }
        }
    }
    public bool ShowValueText {
        get { return valueText.gameObject.activeSelf; }
        set {
            valueText.gameObject.SetActive(value);
        }
    }
    
    float fValue, prevValue, maxValue = 100;
    void UpdateProgressCore() {
        if (MaxValue > 0) {
            if (HideWhenFull) gameObject.SetActive(Value != MaxValue);
            if (EnableValueChangeAnimation)
                StartCoroutine(AnimateValueChange());
            else
                UpdateProgress(Value);
            if (EnableTextChangeAnimation)
                StartCoroutine(AnimateValueTextChange());
            else
                valueText.text = Value.ToString();
        }
    }
    protected virtual void UpdateProgress(float value) {
        float width = barRectTransform.rect.width * (value / MaxValue);
        progressImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
    IEnumerator AnimateValueChange() {
        float timer = 0;
        while (timer < animationDuration) {
            timer += Time.deltaTime;
            float animRatio = timer / animationDuration;
            float animProgress = MathUtils.EaseOutCubic(animRatio);
            UpdateProgress(prevValue + (fValue - prevValue) * animProgress);
            yield return null;
        }
    }
    IEnumerator AnimateValueTextChange() {
        float timer = 0;
        while (timer < animationDuration) {
            timer += Time.deltaTime;
            float animRatio = timer / animationDuration;
            float animProgress = MathUtils.EaseOutCubic(animRatio);
            float animValue = Mathf.Round(prevValue + (fValue - prevValue) * animProgress);
            valueText.text = animValue.ToString();
            yield return null;
        }
    }
    void LateUpdate() {
        // face HealthBar to camera
        // transform.parent.LookAt(Camera.main.transform);
    }
}
