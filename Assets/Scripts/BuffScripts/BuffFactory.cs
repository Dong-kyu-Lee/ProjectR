using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFactory
{
    private GameObject targetObject;    //적용 대상 오브젝트
    delegate Buff BuffConstructor(float duration, GameObject targetObject);
    private List<BuffConstructor> constructorList = new List<BuffConstructor>();

    //버프팩토리(팩토리 패턴)
    public BuffFactory(GameObject target)
    {
        targetObject = target;
        GenerateBuffConstructorList();
    }

    private void GenerateBuffConstructorList()
    {
        //BuffType 열거형 순서에 맞게 델리게이트 생성자들을 추가할 것.
        constructorList.Add((duration, targetObject) => new AtkDmgIncBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new DmgReductIncBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new BlessBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new RagingBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new CritDmgIncBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new CritPercentIncBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new PriceAdditionalIncBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new AttackSpeedIncBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new MoveSpeedIncBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new ExtremeSpeedBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new EagleEyeBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new BulkUpBuff(duration, targetObject));
        constructorList.Add((duration, targetObject) => new IronBody(duration, targetObject));
    }

    public Buff GenerateBuff(BuffType type, float duration = 0.0f)
    {
        return constructorList[(int)type](duration, targetObject);
        
        /* 버프 생성 메서드를 델리게이트와 람다 써서 했긴 했는데 혹시 오류날까봐 구버전 코드를 주석처리함.
        Buff buff = constructorList[(int)type](duration, targetObject);
        switch (type) 
        {
            case BuffType.AttackDamageIncrease:
                buff = new AtkDmgIncBuff(duration, targetObject);
                break;
            case BuffType.DamageReductionIncrease:
                buff = new DmgReductIncBuff(duration, targetObject);
                break;
            case BuffType.Bless:
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
            case BuffType.PriceAdditionalIncrease:
                buff = new PriceAdditionalIncBuff(duration, targetObject);
                break;
            case BuffType.AttackSpeedIncrease:
                buff = new AttackSpeedIncBuff(duration, targetObject);
                break;
            case BuffType.MoveSpeedIncrease:
                buff = new MoveSpeedIncBuff(duration, targetObject);
                break;
            case BuffType.ExtremeSpeed:
                buff = new ExtremeSpeedBuff(duration, targetObject);
                break;
            case BuffType.EagleEye:
                buff = new EagleEyeBuff(duration, targetObject);
                break;
            case BuffType.BulkUp:
                buff = new BulkUpBuff(duration, targetObject);
                break;
            case BuffType.IronBody:
                buff = new IronBody(duration, targetObject);
                break;
            default:
                throw new Exception("Invalid Buff Type");
        }
        return buff;
        */
    }
}
