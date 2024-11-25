using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    attackDamageIncrease,
}

public abstract class Buff
{
    protected float currentDuration;        //남은 버프 지속시간.
    protected GameObject targetObject;      //버프 적용 대상
    protected int currentBuffLevel = 0;     //버프 중첩 횟수.
    protected int maxBuffLevel = 3;

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

    //버프를 target에게 적용시키는 메서드.
    public abstract void ApplyBuffEffect();

    //버프를 중첩시키는 메서드
    public abstract void BuffOverlap(float duration);
    
    //target에게 적용된 버프를 제거하는 메서드
    public abstract void RemoveBuffEffect();

    //버프를 업그레이드 후 타겟에게 적용시키는 메서드
    protected abstract void BuffUpgrade();
}
