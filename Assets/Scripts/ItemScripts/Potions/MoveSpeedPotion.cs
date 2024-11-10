using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedPotion : Potion
{
    [SerializeField] 
    private float moveSpeedIncreasement = 10;   //이동속도 증가량 (퍼센트)
    public float MoveSpeedIncreasement { get { return moveSpeedIncreasement; } }

    //이동속도 증가 효과 발생
    public override void DoEffect(PlayerStatus playerStatus)
    {
        float forDebug = playerStatus.MoveSpeed; //디버깅용
        playerStatus.MoveSpeed += playerStatus.MoveSpeed * moveSpeedIncreasement * 0.01f;
        //playerStatus.AdditionalMoveSpeed += moveSpeedIncreasement;
        Debug.Log($"이동속도 {moveSpeedIncreasement}% 증가함 : {forDebug} -> {playerStatus.MoveSpeed}");
    }
}
