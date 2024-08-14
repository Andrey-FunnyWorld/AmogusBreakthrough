using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScoreText))]
public class ScoreTextSound : MonoBehaviour {
    public AudioSource AudioSource;
    ScoreText scoreText;
    public void Play() {
        scoreText = GetComponent<ScoreText>();
        AudioSource.Play();
        StartCoroutine(Utils.WaitAndDo(scoreText.AnimationDuration, () => {
            AudioSource.Stop();
        }));
    }
}
