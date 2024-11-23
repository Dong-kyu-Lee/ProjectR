using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    private float timeRemaining;
    private float maxTime;

    public float CurrentTime { get { return timeRemaining; } set { timeRemaining = value; } }
    public float MaxTime { get { return maxTime; } set { maxTime = value; } }

    public abstract void EnableBuff(PlayerStatus status, float duration);
    public abstract void DisableBuff();
    public bool isBuffActive()
    {
        return 0.0f < timeRemaining;
    }



}
