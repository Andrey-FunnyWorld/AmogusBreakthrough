using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesintegratorAnimationHelper : MonoBehaviour {
    public AudioSource AudioSource;
    public AudioClip Acceleration, Bang, Fail;
    public Desintegrator Desintegrator;
    public void BangAction() {
        AudioSource.clip = Bang;
        AudioSource.Play();
        Desintegrator.Bang();
    }
    public void Accelerate() {
        AudioSource.clip = Acceleration;
        AudioSource.Play();
    }
    public void FailAction() {
        AudioSource.clip = Fail;
        AudioSource.Play();
    }
    public void Finish() {
        Desintegrator.FinishAttempt();
    }
}
