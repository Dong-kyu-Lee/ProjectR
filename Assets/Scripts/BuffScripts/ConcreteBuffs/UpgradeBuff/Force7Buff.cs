using UnityEngine;

public class Force7Buff : Buff
{
    private float atkDmgIncGap = 5f;     //피해량 증가량
    public Force7Buff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Force7;
        maxBuffLevel = 4;
        maxDuration = 20.0f;
    }

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.Damage += atkDmgIncGap;
        currentBuffValue["atkDmgInc"] = atkDmgIncGap * (CurrentBuffLevel + 1);
    }

    //적용된 버프를 해제하는 함수. 각 스탯마다 누적된 값을 계산해 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.Damage -= atkDmgIncGap * (CurrentBuffLevel + 1);
    }
}