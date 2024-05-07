using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    [SerializeField] private float cooldownSec;
    private float timesUp;

    public Cooldown() { }

    public Cooldown(float value)
    {
        cooldownSec = value;
    }

    public float Value
    {
        get => cooldownSec;
        set => cooldownSec = value;
    }

    public bool IsReady => timesUp <= Time.time;

    public float TimeLast => Mathf.Max(timesUp - Time.deltaTime, 0);

    public void Reset()
    {
        timesUp = Time.time + cooldownSec;
    }
}
