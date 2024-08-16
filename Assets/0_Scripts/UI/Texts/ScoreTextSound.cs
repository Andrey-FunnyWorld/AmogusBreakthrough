using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScoreText))]
public class ScoreTextSound : MonoBehaviour {
    public AudioSource AudioSource;
    ScoreText scoreText;
    public void Play() {
        Debug.Log("ScoreTextSound Play");
        AudioSource.Play();
        scoreText = GetComponent<ScoreText>();
        StartCoroutine(Utils.WaitAndDo(scoreText.AnimationDuration, () => {
            AudioSource.Stop();
        }));
    }
}
