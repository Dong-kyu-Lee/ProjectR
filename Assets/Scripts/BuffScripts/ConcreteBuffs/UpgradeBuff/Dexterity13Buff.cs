using UnityEngine;

public class Dexterity13Buff : Buff
{
    private float atkSpeedIncGap = 0.02f;     //공격속도 증가량
    
    public Dexterity13Buff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 50;
        maxDuration = 8.0f;
    }

    //대상에게 버프를 적용하는 함수. 각 스탯이 누적되며 증가하는 식
    public override void ApplyBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.AdditionalAttackSpeed += atkSpeedIncGap;
        Debug.Log("현재 레벨" + currentBuffLevel);
        Debug.Log("지속시간" + currentDuration);
        Debug.Log(playerStatus.TotalAttackSpeed);
    }

    //적용된 버프를 해제하는 함수. 각 스탯마다 누적된 값을 계산해 감소하는 식
    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = targetObject.GetComponent<PlayerStatus>();

        playerStatus.AdditionalAttackSpeed -= atkSpeedIncGap * (currentBuffLevel + 1);
    }
}
