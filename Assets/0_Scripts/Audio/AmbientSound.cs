using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour {
    public AudioSource AudioSource;
    public AudioClip[] Clips;
    public float PauseDurationStart;
    public float PauseDurationEnd;
    Coroutine coroutine;
    void Start() {
        coroutine = StartCoroutine(RandomSounds());
    }
    IEnumerator RandomSounds() {
        float time = Random.Range(PauseDurationStart, PauseDurationEnd);
        float timer = 0;
        while (timer < time) {
            timer += Time.deltaTime;
            yield return null;
        }
        AudioSource.clip = Clips[Random.Range(0, Clips.Length)];
        AudioSource.volume = AudioSource.clip.name.Contains("weld") ? 0.9f : 0.2f;
        AudioSource.Play();
        coroutine = StartCoroutine(RandomSounds());
    }
}
