using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRandomShot : MonoBehaviour {
    public AudioSource AudioSource;
    public AudioClip[] Clips;
    public void Play() {
        AudioSource.clip = Clips[Random.Range(0, Clips.Length)];
        AudioSource.Play();
    }
}
