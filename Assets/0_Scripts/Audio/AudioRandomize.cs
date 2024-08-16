using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRandomize : MonoBehaviour {
    public AudioSource AudioSource;
    public AudioClip[] Clips;
    Coroutine audioCoroutine;
    bool stopNextRoutine = false;
    public void Play() {
        AudioSource.clip = GetNextSound();
        AudioSource.Play();
        //audioCoroutine = StartCoroutine(NextSound());
    }
    public void Stop(bool immediate) {
        if (immediate) {
            if (audioCoroutine != null)
                StopCoroutine(audioCoroutine);
            AudioSource.Stop();
        } else {
            stopNextRoutine = true;
        }
    }
    IEnumerator NextSound() {
        while (AudioSource.isPlaying) {
            yield return null;
        }
        if (!stopNextRoutine) {
            AudioSource.Stop();
            AudioSource.clip = GetNextSound();
            AudioSource.Play();
            audioCoroutine = StartCoroutine(NextSound());
        }
    }
    AudioClip GetNextSound() {
        return Clips[Random.Range(0, Clips.Length)];
    }
}
