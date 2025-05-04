using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkenDeBuff : Buff
{
    private float[] moveSpeedDecGap = { 0.1f, 0.2f, 0.2f };         //이동속도 감소량의 간격
    private float[] attackSpeedDecGap = { 10.0f, 10.0f, 30.0f };    //공격속도 감소량의 간격

    public DrunkenDeBuff(float duration, GameObject target) : base(duration, target)
    {
        this.BuffType = BuffType.Drunken;
        maxBuffLevel = 3;
    }

    public override void ApplyBuffEffect()
    {
        if (currentBuffLevel < maxBuffLevel - 1)
        {
            Debug.Log("출혈 디버프 활성화"); 
            PlayerStatus playerStatus = GetPlayerStatus();
            if (playerStatus == null)
                return;
            playerStatus.MoveSpeed -= moveSpeedDecGap[currentBuffLevel];
            playerStatus.AttackSpeed -= attackSpeedDecGap[currentBuffLevel];
        }
        else
        {
            ActivateSleepBuff();
        }
        Debug.Log("만취 디버프 부여");
    }
    private void ActivateSleepBuff()
    {
        if (BuffManager.Instance != null)
        {
            BuffManager.Instance.ActivateBuff(BuffType.Sleep, 10.0f);
            Debug.Log("Sleep 디버프가 활성화되었습니다.");
        }
        else
        {
            Debug.LogWarning("BuffManager가 null입니다.");
        }
    }

    public override void RemoveBuffEffect()
    {
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return;
        playerStatus.MoveSpeed += GetCurrentSumOfArray(moveSpeedDecGap);
        playerStatus.AttackSpeed += GetCurrentSumOfArray(attackSpeedDecGap);
        Debug.Log("만취 디버프 해제");
    }
}
