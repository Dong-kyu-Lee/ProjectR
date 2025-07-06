using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzedDeBuff : Buff
{
    public BuzzedDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Buzzed;
        maxDuration = 5;
        maxBuffLevel = 10;
    }

    public override void ApplyBuffEffect()
    {
        if (currentBuffLevel >= maxBuffLevel - 1)
        {
            ActivateDrunkenBuff();
        }
    }

    //스택이 10스택 이상 쌓일 경우 만취 디버프로 전환
    private void ActivateDrunkenBuff()
    {
        BuffManager targetBuffManager = targetObject.GetComponent<BuffManager>();
        targetBuffManager.ActivateBuff(BuffType.Drunken);
        targetBuffManager.DeactivateBuff(BuffType.Buzzed);
    }

    public override void RemoveBuffEffect()
    {
    }
}
