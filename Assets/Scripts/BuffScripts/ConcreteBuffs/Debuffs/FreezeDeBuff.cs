using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerObj;

public class FreezeDeBuff : Buff
{
    private float moveSpeedDec = 0.5f;      //이동속도 감소시킬 양
    private float speedDecAmount = 0.0f;    //이동속도 실질적으로 감소한 양을 기록하는 변수

    public FreezeDeBuff(float duration, GameObject target) : base(duration, target) 
    {
        maxBuffLevel = 5;
    }

    public override void ApplyBuffEffect()
    {
        if(currentBuffLevel == 0)
        {
            PlayerStatus playerStatus = GetPlayerStatus();
            if (playerStatus == null)
                return;
            speedDecAmount = playerStatus.MoveSpeed * moveSpeedDec;
            playerStatus.MoveSpeed -= speedDecAmount;
            Debug.Log("빙결 디버프 : 이동속도 감소");
        }
        else if(currentBuffLevel == 4)
        {   //5번째 스택일 때 데미지 적용
            PlayerStatus playerStatus = GetPlayerStatus();
            if (playerStatus == null)
                return;
            playerStatus.Hp -= playerStatus.Hp * 0.3f;
            currentBuffLevel--;
            Debug.Log("빙결 디버프 : 데미지 적용");
        }
        else
        {
            Debug.Log("빙결 디버프 : 스택 증가 " + currentBuffLevel);
        }
    }

    public override void RemoveBuffEffect()
    {
        //버프가 해제되면 실질적으로 감소한 양만큼만 롤백시킴
        PlayerStatus playerStatus = GetPlayerStatus();
        if (playerStatus == null)
            return;
        playerStatus.MoveSpeed += speedDecAmount;
    }
}
