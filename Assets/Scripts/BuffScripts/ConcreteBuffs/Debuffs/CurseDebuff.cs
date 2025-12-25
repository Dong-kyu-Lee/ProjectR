using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseDeBuff : Buff
{
    private float damageTakenInc = 0.03f;
    private float totalDamageTaken = 0;

    public CurseDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Curse;
        maxDuration = 10;
        maxBuffLevel = 5;
        totalDamageTaken = 0;
        if (CalcDamage.Instance.curseEffect13) damageTakenInc = 0.1f;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;
        targetStatus.DamageTaken -= totalDamageTaken;
        if (CalcDamage.Instance.curseEffect13) damageTakenInc = 0.1f;
        totalDamageTaken = damageTakenInc * (currentBuffLevel + 1);
        targetStatus.DamageTaken += totalDamageTaken;
        Debug.Log(currentBuffLevel + " " + targetStatus.DamageTaken);
    }

    public override void RenewBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;
        targetStatus.DamageTaken -= totalDamageTaken;
        if (CalcDamage.Instance.curseEffect13) damageTakenInc = 0.1f;
        totalDamageTaken = damageTakenInc * (currentBuffLevel + 1);
        targetStatus.DamageTaken += totalDamageTaken;
        Debug.Log(currentBuffLevel + " " + targetStatus.DamageTaken);
    }

    public override void RemoveBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.DamageTaken -= totalDamageTaken;
    }
}