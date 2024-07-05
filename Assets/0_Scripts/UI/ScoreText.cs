using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour {
    public TMP_Text Text;
    public bool AnimateValueChange = true;
    const float ANIMATION_DURATION = 0.7f;
    int score = 0;
    int prevValue = 0;
    string format = "{0} <sprite name=\"coin\">";
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
        if (AnimateValueChange)
            StartCoroutine(AnimateValueTextChange());
        else
            Text.text = string.Format(format, score);
    }
    IEnumerator AnimateValueTextChange() {
        float timer = 0;
        while (timer < ANIMATION_DURATION) {
            timer += Time.deltaTime;
            float animRatio = timer / ANIMATION_DURATION;
            float animProgress = MathUtils.EaseOutCubic(animRatio);
            float animValue = Mathf.Round(prevValue + (score - prevValue) * animProgress);
            Text.text = string.Format(format, animValue);
            yield return null;
        }
    }
    public void SetScoreSilent(int newScore) {
        bool prevAnimateValueChange = AnimateValueChange;
        AnimateValueChange = false;
        Score = newScore;
        AnimateValueChange = prevAnimateValueChange;
    }
}
