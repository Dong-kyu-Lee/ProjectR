using UnityEngine;

public class Critical7Buff : Buff
{
    private float critPerInc = 1f;     //크리티컬 확률 증가량

    public Critical7Buff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 1;
        maxDuration = 1.0f;
    }

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.CriticalPercent += critPerInc;
    }

    //적용된 버프를 해제하는 함수. 각 스탯마다 누적된 값을 계산해 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.CriticalPercent -= critPerInc;
    }
}
