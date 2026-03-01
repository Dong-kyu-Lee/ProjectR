using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDebuff
{
    public float amount; // 0.3 = 30% 감속
    public float endTime; // 만료 시각

    public SlowDebuff(float amount, float duration)
    {
        this.amount = amount;
        this.endTime = Time.time + duration;
    }
}
