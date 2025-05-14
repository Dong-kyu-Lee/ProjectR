using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDeBuff : Buff
{
    public override BuffType BuffType => BuffType.Poision;
    private float[] poisonDmg = { 1.0f, 2.0f, 5.0f }; // 레벨별 틱당 데미지

    private float tickTimer = 0f; // 틱 타이머

    public PoisonDeBuff(float duration, GameObject target) : base(duration, target) { }

    public override void ApplyBuffEffect()
    {
        Status targetStatus = targetObject.GetComponent<Status>();
        if (targetStatus == null) return;

        targetStatus.Hp -= poisonDmg[currentBuffLevel];
        Debug.Log($"[Poison] HP 감소: -{poisonDmg[currentBuffLevel]}, 현재 HP: {targetStatus.Hp}");

        if (InGameUIManager.Instance != null)
            InGameUIManager.Instance.CheckHp();
    }
    public override void DoActionOnActivate(float deltaTime)
    {
        tickTimer += deltaTime;

        if (tickTimer >= 1f) // 1초마다 데미지 적용
        {
            ApplyBuffEffect();
            tickTimer = 0f; // 타이머 리셋
        }

        base.DoActionOnActivate(deltaTime); // 지속시간 감소 처리
    }
    public override void RemoveBuffEffect()
    {
        Debug.Log("Poison 디버프 해제");
    }
}
