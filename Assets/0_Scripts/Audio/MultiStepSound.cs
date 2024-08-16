using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiStepSound : MonoBehaviour {
    public AudioSource AudioSource;
    public AudioClip Start, Middle, End;
    Coroutine coroutine;
    bool passToEnd = false;
    public void Play() {
        coroutine = StartCoroutine(OrchestrateSound());
    }
    public void Stop() {
        passToEnd = true;
    }
    IEnumerator OrchestrateSound() {
        AudioSource.clip = Start;
        AudioSource.loop = false;
        AudioSource.Play();
        while (AudioSource.isPlaying) {
            yield return null;
        }
        AudioSource.Stop();
        AudioSource.clip = Middle;
        AudioSource.loop = true;
        AudioSource.Play();
        while (!passToEnd) {
            yield return null;
        }
        AudioSource.Stop();
        AudioSource.clip = End;
        AudioSource.loop = false;
        AudioSource.Play();
    }
}
