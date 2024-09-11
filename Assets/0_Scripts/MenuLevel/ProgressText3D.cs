using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class ProgressText3D : MonoBehaviour {
    public SkinType TextType;
    public TextMeshPro Text;
    public Transform ProgressBar;
    public float MaxSize;
    public float AnimationDuration = 0.7f;
    int percentValue = 0;
    Coroutine animationRoutine;
    
    public void SetProgress(int percent, string ratioText = "") {
        string text = TextType == SkinType.Hat ? LocalizationKeys.HatsProgress : LocalizationKeys.BackpackProgress;
        Text.text = string.Format(MyLocalization.Instance.GetLocalizedText(text), ratioText == "" ? percent : ratioText);
        percentValue = percent;
        SetProgressBar(percent);
    }
    void SetProgressBar(int percent) {
        float progressSize = MaxSize * percent / 100;
        Vector3 scale = new Vector3(progressSize, ProgressBar.localScale.y, ProgressBar.localScale.z);
        ProgressBar.localScale = scale;
    }
    public void AnimateProgress(bool startForward) {
        animationRoutine = StartCoroutine(Animate(AnimationDuration, startForward));
    }
    public void StopAnimation() {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);
        SetProgressBar(percentValue);
    }
    IEnumerator Animate(float time, bool startForward = true) {
        float timer = 0;
        bool forward = startForward;
        float start = 0, end = 100;
        while (true) {
            timer += Time.deltaTime;
            float value = (forward ? start : end) + (forward ? 1f : -1f) * (end - start) * timer / time;
            SetProgressBar((int)Mathf.Floor(value));
            if (timer > time) {
                forward = !forward;
                timer = 0;
            }
            yield return null;
        }
    }
}