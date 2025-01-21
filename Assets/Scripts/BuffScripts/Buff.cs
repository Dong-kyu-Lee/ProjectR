using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    AttackDamageIncrease,       //공격력 증가
    DamageReductionIncrease,    //피해 감소량 증가
    Bless,                      //축복 버프
    Raging,                     //광분 버프
    CritDamageIncrease,         //크리티컬 데미지 증가
    CritPercentIncrease,        //크리티컬 확률 증가
    PriceAdditionalIncrease,     //재화 획득량 증가
}

public abstract class Buff
{
    protected float currentDuration;        //남은 버프 지속시간
    protected GameObject targetObject;      //버프 적용 대상
    protected int currentBuffLevel = 0;     //현재 버프 레벨 (0 ~ maxBuffLevel - 1 의 값을 가짐)
    protected int maxBuffLevel = 3;         //최대 버프 레벨

    public float CurrentDuration { 
        get { return currentDuration; }
        set
        {
            currentDuration = value;
            if (currentDuration < 0.0f) currentDuration = 0.0f;
        }
    }
    public int CurrentBuffLevel
    { 
        get { return currentBuffLevel; } 
        set 
        {
            currentBuffLevel = value;
            if(currentBuffLevel < 0) currentBuffLevel = 0;
        } 
    }

    public Buff(float duration, GameObject target)
    {
        currentDuration = duration;
        targetObject = target;
    }

    //특정 스탯 증가량을 대상에게 적용시키는 메서드
    public abstract void ApplyBuffEffect();

    //대상에게 적용된 스탯증가량을 제거하는 메서드
    public abstract void RemoveBuffEffect();

    //버프를 중첩시키는 메서드
    public virtual void BuffOverlap(float duration)
    {
        if (currentBuffLevel < maxBuffLevel - 1)
        {
            currentBuffLevel++;
            ApplyBuffEffect();
        }
        currentDuration += duration;
    }

    //버프가 지속되는 동안 효과를 정의하는 메서드
    public virtual void DoActionOnActivate(float tickDuration = 1.0f) 
    {
        CurrentDuration -= tickDuration;
    }

    //현재 버프 레벨까지의 특정 스탯 증감 누적량을 계산해주는 함수
    protected float GetCurrentSumOfArray(float[] array)
    {
        float sum = 0.0f;
        for (int i = 0; i < currentBuffLevel + 1; i++)
        {
            sum += array[i];
        }
        return sum;
    }
}