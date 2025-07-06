using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Buff
{
    protected float maxDuration = 10.0f;            //최대 버프 지속시간
    protected float currentDuration = 0.0f;         //현재 버프 지속시간
    protected float buffEffectTick = 1.0f;          //다음 버프 효과 적용시간 까지의 간격
    protected GameObject targetObject;              //버프 적용 대상
    protected int currentBuffLevel = 0;     //현재 버프 레벨 (0 ~ maxBuffLevel - 1 의 값을 가짐)
    protected int maxBuffLevel = 3;         //최대 버프 레벨

    public BuffType BuffType { get; protected set; }
    public float MaxDuration
    {
        get { return maxDuration * (1 + CalcDamage.Instance.additionalDebuffTime); }
    }
    public float CurrentDuration
    {
        get { return currentDuration; }
        set
        {
            currentDuration = (value < MaxDuration) ? value : MaxDuration;
        }
    }
    public int CurrentBuffLevel
    {
        get { return currentBuffLevel; }
        set
        {
            currentBuffLevel = (value < 0) ? 0 : value;
        }
    }

    public Buff(float duration, GameObject target)
    {
        currentDuration = duration;
        targetObject = target;
    }

    //버프가 활성화 될때 해야할 일을 지정하는 메서드
    public abstract void ApplyBuffEffect();

    //버프가 갱신 될 때 해야할 일을 지정하는 메서드
    public virtual void RenewBuffEffect() { }

    //버프가 비활성화 될 때 해야할 일을 지정하는 메서드
    public abstract void RemoveBuffEffect();

    //버프를 중첩시키고 효과를 적용하는 메서드
    public virtual void BuffOverlap(float duration)
    {
        if (currentBuffLevel < maxBuffLevel - 1)
        {
            currentBuffLevel++;
            ApplyBuffEffect();
        }
        else RenewBuffEffect();
        CurrentDuration += duration;
    }

    //버프가 지속되는 동안 해야할 일을 정의하는 메서드
    public virtual void DoActionOnActivate(float tickDuration = 1.0f)
    {
        CurrentDuration -= tickDuration;
        //버프 적용 주기가 1.0초인 버프가 0.2초 남았는데도 로직에 의해 효과를 한번 더 받는 경우를 고려해야할 필요가 있음.
        //버프 지속 시간들이 항상 버프 적용 주기의 배수임을 보장하면 이 부분은 고려 안해도 됨.
        /*if(currentDuration > 1.0f)
        {
            ApplyBuffEffect();
        }*/
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