using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour {
    public TMP_Text Text;
    public bool AnimateValueChange = true;
    public string Format = "{0} <sprite name=\"coin\">";
    public float AnimationDuration = 0.7f;
    public AudioSource AnimationAudio;
    int score = 0;
    int prevValue = 0;
    void Awake() {
        Text.text = string.Format(Format, score);
    }
    public int Score {
        get { return score;}
        set {
            if (score != value) {
                prevValue = score;
                score = value;
                ScoreChanged();
            }
        }
    }
    void ScoreChanged() {
        if (AnimateValueChange) {
            if (gameObject.activeInHierarchy)
                StartCoroutine(AnimateValueTextChange());
            else
                Text.text = string.Format(Format, score);
        }
        else
            Text.text = string.Format(Format, score);
    }
    IEnumerator AnimateValueTextChange() {
        if (AnimationAudio != null)
            AnimationAudio.Play();
        float timer = 0;
        while (timer < AnimationDuration) {
            timer += Time.deltaTime;
            float animRatio = timer / AnimationDuration;
            float animProgress = MathUtils.EaseOutCubic(animRatio);
            float animValue = Mathf.Round(prevValue + (score - prevValue) * animProgress);
            Text.text = string.Format(Format, animValue);
            yield return null;
        }
        if (AnimationAudio != null)
            AnimationAudio.Stop();
    }
    public void SetScoreSilent(int newScore) {
        bool prevAnimateValueChange = AnimateValueChange;
        AnimateValueChange = false;
        Score = newScore;
        AnimateValueChange = prevAnimateValueChange;
    }
}
