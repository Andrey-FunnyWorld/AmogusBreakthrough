using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimerSyncer : MonoBehaviour {
    public AdSpinButton[] AdButtons;
    float timer = 0;
    void Update() {
        timer += Time.deltaTime;
        if (timer > 1) {
            timer = 0;
            Tick();
        }
    }
    void Tick() {
        DateTime max = AdButtons.Select(b => b.DateAvailable).Max();
        foreach (AdSpinButton button in AdButtons) {
            if (button.DateAvailable < max) button.ApplyFinishDate(max);
            button.TimerTick();
        }
    }
}
