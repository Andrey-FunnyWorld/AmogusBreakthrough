using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ImposterUI : MonoBehaviour {
    public HitRangeBar HitRangeBar;
    public Transform HitRangeUI;
    public HitRangeBarParams[] StepParams;
    public Animator Animator;
    public Image TransitionImage;
    public float TransitionDuration = 0.5f;
    const string ANIMATION_TRIGGER_INTRO = "intro";
    const string ANIMATION_TRIGGER_FULLINTRO = "fullintro";
    const string ANIMATION_TRIGGER_HELP = "help";
    const string ANIMATION_TRIGGER_SUCCESS = "success";
    const string ANIMATION_TRIGGER_FAIL = "fail";
    const float PAUSE_BETWEEN_HIT_TESTS = 0.5f;
    int stepIndex = 0;
    int maxSteps = 3;
    void RunHitBar(int index) {
        HitRangeBar.SetParams(StepParams[index].ThumbSpeed, StepParams[index].TargetRatio);
        HitRangeBar.RunThumb();
    }
    public void RunBar() {
        HitRangeUI.gameObject.SetActive(true);
        RunHitBar(stepIndex);
    }
    public void Intro(bool full = false) {
        string trigger = full ? ANIMATION_TRIGGER_FULLINTRO : ANIMATION_TRIGGER_INTRO;
        Animator.SetTrigger(trigger);
    }
    public void ShowHelp() {
        Animator.SetTrigger(ANIMATION_TRIGGER_HELP);
    }
    public void HitTestRangeBar(bool isHit) {
        Animator.SetTrigger(isHit ? ANIMATION_TRIGGER_SUCCESS : ANIMATION_TRIGGER_FAIL);
        StartCoroutine(Utils.WaitAndDo(PAUSE_BETWEEN_HIT_TESTS, () => {
            stepIndex++;
            if (stepIndex < maxSteps) {
                RunHitBar(stepIndex);
            } else {
                StartCoroutine(Utils.WaitAndDo(PAUSE_BETWEEN_HIT_TESTS, () => {
                    Animator.Rebind();
                    Animator.Update(0f);
                    HitRangeUI.gameObject.SetActive(false);
                }));
            }
        }));
    }
    public int GetStep() {
        return stepIndex;
    }
    public void Init(Team team) {
        maxSteps = Mathf.Min(team.MatesCount - 1, maxSteps);
    }
    public void Transition(bool show) {
        TransitionImage.gameObject.SetActive(true);
        StartCoroutine(TranslateColor(show));
    }
    IEnumerator TranslateColor(bool show) {
        float timer = 0;
        while (timer < TransitionDuration) {
            timer += Time.deltaTime;
            float alpha = show ? timer / TransitionDuration : 1 - timer / TransitionDuration;
            TransitionImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        TransitionImage.color = new Color(0, 0, 0, show ? 1 : 0);
        if (!show) TransitionImage.gameObject.SetActive(false);
    }
    public void HandleDropResult(bool success) {
        // hide ui?
        // show fail/success panel
    }
}

[System.Serializable]
public class HitRangeBarParams {
    public float ThumbSpeed;
    public float TargetRatio;
}