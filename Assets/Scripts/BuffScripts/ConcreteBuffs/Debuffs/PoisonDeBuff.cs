using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDeBuff : Buff
{
    private float[] poisonDmg = { 1f, 2f, 3f }; // 레벨별 틱당 독 데미지

    public PoisonDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Poison;
        maxDuration = 10;
        isDebuff = true;
    }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        CalcReceiveDamage.Instance.TakeDebuffDamage(poisonDmg[currentBuffLevel], targetStatus, false);
        BuffEffectManager.Instance.PlayBuffEffect(this.BuffType, targetObject.transform.position, true);
        Debug.Log(currentDuration + "/" + MaxDuration);
    }

    public override void DoActionOnActivate(float tickDuration)
    {
        base.DoActionOnActivate(tickDuration);
        ApplyBuffEffect();
    }

    public override void RemoveBuffEffect()
    {
    }
}
