using System;
using UnityEngine;

public class BuffFactory
{
    private GameObject targetObject;    //적용 대상 오브젝트
    delegate Buff BuffConstructor(float duration, GameObject targetObject);
    private BuffConstructor[] buffConstructors = new BuffConstructor[Enum.GetValues(typeof(BuffType)).Length];

    //버프팩토리(팩토리 패턴)
    public BuffFactory(GameObject target)
    {
        targetObject = target;
        GenerateBuffConstructorList();
    }

    private void GenerateBuffConstructorList()
    {
        //BuffType 열거형 순서에 맞게 델리게이트 생성자들을 추가할 것.
        buffConstructors[(int)BuffType.AttackDamageIncrease] = (duration, targetObject) => new AtkDmgIncBuff(duration, targetObject);
        buffConstructors[(int)BuffType.DamageReductionIncrease] = (duration, targetObject) => new DmgReductIncBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Bless] = (duration, targetObject) => new BlessBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Raging] = (duration, targetObject) => new RagingBuff(duration, targetObject);
        buffConstructors[(int)BuffType.CritDamageIncrease] = (duration, targetObject) => new CritDmgIncBuff(duration, targetObject);
        buffConstructors[(int)BuffType.CritPercentIncrease] = (duration, targetObject) => new CritPercentIncBuff(duration, targetObject);
        buffConstructors[(int)BuffType.PriceAdditionalIncrease] = (duration, targetObject) => new PriceAdditionalIncBuff(duration, targetObject);
        buffConstructors[(int)BuffType.AttackSpeedIncrease] = (duration, targetObject) => new AttackSpeedIncBuff(duration, targetObject);
        buffConstructors[(int)BuffType.MoveSpeedIncrease] = (duration, targetObject) => new MoveSpeedIncBuff(duration, targetObject);
        buffConstructors[(int)BuffType.ExtremeSpeed] = (duration, targetObject) => new ExtremeSpeedBuff(duration, targetObject);
        buffConstructors[(int)BuffType.EagleEye] = (duration, targetObject) => new EagleEyeBuff(duration, targetObject);
        buffConstructors[(int)BuffType.BulkUp] = (duration, targetObject) => new BulkUpBuff(duration, targetObject);
        buffConstructors[(int)BuffType.IronBody] = (duration, targetObject) => new IronBodyBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Reflection] = (duration, targetObject) => new ReflectionBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Invincible] = (duration, targetObject) => new InvincibleBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Critical4] = (duration, targetObject) => new Critical4Buff(duration, targetObject);
        buffConstructors[(int)BuffType.Critical7] = (duration, targetObject) => new Critical7Buff(duration, targetObject);
        buffConstructors[(int)BuffType.Dexterity7] = (duration, targetObject) => new Dexterity7Buff(duration, targetObject);
        buffConstructors[(int)BuffType.Dexterity13] = (duration, targetObject) => new Dexterity13Buff(duration, targetObject);
        buffConstructors[(int)BuffType.Poision] = (duration, targetObject) => new PoisonDeBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Slow] = (duration, targetObject) => new SlowDeBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Freeze] = (duration, targetObject) => new FreezeDeBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Sleep] = (duration, targetObject) => new SleepDeBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Drunken] = (duration, targetObject) => new DrunkenDeBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Bleeding] = (duration, targetObject) => new BleedingDebuff(duration, targetObject);
        buffConstructors[(int)BuffType.Stun] = (duration, targetObject) => new StunDeBuff(duration, targetObject);
        buffConstructors[(int)BuffType.StoneCurse] = (duration, targetObject) => new StoneCurseDeBuff(duration, targetObject);
        buffConstructors[(int)BuffType.Confusion] = (duration, targetObject) => new ConfusionDeBuff(duration, targetObject);
    }

    public Buff GenerateBuff(BuffType type, float duration = 0.0f)
    {
        if (buffConstructors[(int)type] == null)
        {
            throw new Exception("Invalid Buff Type : " + type.ToString());
        }
        return buffConstructors[(int)type](duration, targetObject);
        
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
