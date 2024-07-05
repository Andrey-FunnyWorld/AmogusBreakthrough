using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextMenu : ScoreText {
    public Animator Animator;
    public void HighlightError() {
        Animator.SetTrigger("error");
    }
}
