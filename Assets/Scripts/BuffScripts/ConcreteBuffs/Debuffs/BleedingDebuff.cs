using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedingDebuff : Buff
{
    private float[] bleedingDmg = { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f};    //레벨별 틱당 출혈 데미지

    public BleedingDebuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 5;
    }

    public override void ApplyBuffEffect()
    {
        Debug.Log("출혈 디버프 활성화");
        Status targetStatus = targetObject.GetComponent<Status>();
        CalcReceiveDamage.Instance.TakeTrueDamageQueue(bleedingDmg[currentBuffLevel], targetStatus.gameObject);
        targetStatus.Hp -= bleedingDmg[currentBuffLevel];
        Debug.Log($"출혈 후 플레이어 체력 : {targetStatus.Hp}");
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        ApplyBuffEffect();
        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        Debug.Log("출혈 디버프 비활성화");
    }
}