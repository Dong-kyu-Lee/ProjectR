using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDeBuff : Buff
{
    private float[] moveSpeedDecGap = { 30.0f };
    private float speedDecAmount = 0.0f;

    public SlowDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Slow;
        maxBuffLevel = 1;
    }
    public override void ApplyBuffEffect()
    {
        PlayerStatus temp = targetObject.GetComponent<PlayerStatus>();
        if (temp != null)
        {
            float decPercent = currentBuffLevel < moveSpeedDecGap.Length
                ? moveSpeedDecGap[currentBuffLevel]
                : moveSpeedDecGap[moveSpeedDecGap.Length - 1];

            speedDecAmount = temp.MoveSpeed * decPercent * 0.01f;
            temp.MoveSpeed -= speedDecAmount;
        }
        else
        {
            Debug.LogWarning("SlowDeBuff: PlayerStatus 컴포넌트를 찾을 수 없습니다.");
        }
    }
    public override void RemoveBuffEffect()
    {
        PlayerStatus temp = targetObject.GetComponent<PlayerStatus>();
        if (temp != null)
        {
            temp.MoveSpeed += speedDecAmount;
        }
    }
}
