using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRunTexture : RunningTexture {
    public float StartSpeed = 1;
    void Start() {
        SetSpeed(StartSpeed);
        IsRunning = true;
    }
}
