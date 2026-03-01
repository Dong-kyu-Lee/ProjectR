using UnityEngine;

public class Critical4Buff : Buff
{
    private float moveSpeedInc = 0.3f;     //이동 속도 증가량

    public Critical4Buff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Critical4;
        maxBuffLevel = 1;
        maxDuration = 5.0f;
    }

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed += moveSpeedInc;
    }

    //적용된 버프를 해제하는 함수. 각 스탯마다 누적된 값을 계산해 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus targetStatus = targetObject.GetComponent<PlayerStatus>();
        if (targetStatus == null) return;

        targetStatus.AdditionalMoveSpeed -= moveSpeedInc;
    }
}
