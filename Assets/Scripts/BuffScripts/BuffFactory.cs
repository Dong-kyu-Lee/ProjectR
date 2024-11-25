using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFactory
{
    private GameObject targetObject;    //적용 대상 오브젝트


    //버프팩토리(팩토리 패턴)
    public BuffFactory(GameObject target)
    {
        targetObject = target;
    }

    public Buff GenerateBuff(BuffType type, float duration = 0.0f)
    {

        Buff buff = null;

        switch (type)
        {
            case BuffType.attackDamageIncrease:
                buff = new AtkDmgIncBuff(duration, targetObject);
                break;
            case BuffType.DamageReductionIncrease:
                buff = new DmgReductIncBuff(duration, targetObject);
                break;
            case BuffType.Bless :
                buff = new BlessBuff(duration, targetObject);
                break;
        }

        return buff;
    }

}
