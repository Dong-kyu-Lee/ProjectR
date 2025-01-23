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
        Buff buff;


        switch (type)
        {
            case BuffType.AttackDamageIncrease:
                buff = new AtkDmgIncBuff(duration, targetObject);
                break;
            case BuffType.DamageReductionIncrease:
                buff = new DmgReductIncBuff(duration, targetObject);
                break;
            case BuffType.Bless :
                buff = new BlessBuff(duration, targetObject);
                break;
            case BuffType.Raging:
                buff = new RagingBuff(duration, targetObject);
                break;
            case BuffType.CritDamageIncrease:
                buff = new CritDmgIncBuff(duration, targetObject);
                break;
            case BuffType.CritPercentIncrease:
                buff = new CritPercentIncBuff(duration, targetObject);
                break;
            case BuffType.PriceAdditionalIncrease :
                buff = new PriceAdditionalIncBuff(duration, targetObject);
                break;
            case BuffType.AttackSpeedIncrease:
                buff = new AttackSpeedIncBuff(duration, targetObject);
                break;
            case BuffType.MoveSpeedIncrease:
                buff = new MoveSpeedIncBuff(duration, targetObject);
                break;
            default:
                throw new Exception("Invalid Buff Type");
        }

        return buff;
    }
}
