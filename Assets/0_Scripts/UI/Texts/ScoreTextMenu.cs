using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextMenu : ScoreText {
    public Animator Animator;
    public AudioSource Audio;
    public void HighlightError() {
        Animator.SetTrigger("error");
        Audio.Play();
    }
}
