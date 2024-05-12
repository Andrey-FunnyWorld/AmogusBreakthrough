using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    [SerializeField] private float cooldownSec;
    private float finishTime;

    public Cooldown() { }

    public Cooldown(float value)
    {
        cooldownSec = value;
    }

    public float CooldownValue
    {
        get => cooldownSec;
        set => cooldownSec = value;
    }

    public bool IsReady => finishTime <= Time.time;

    public void Reset()
    {
        finishTime = Time.time + cooldownSec;
    }
}
