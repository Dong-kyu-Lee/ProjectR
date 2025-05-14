using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionDeBuff : Buff
{
    public override BuffType BuffType => BuffType.Confusion;

    public ConfusionDeBuff(float duration, GameObject target) : base(duration, target)
    {
        maxBuffLevel = 1;
    }

    public override void ApplyBuffEffect()
    {
        Debug.Log("혼란 디버프 활성화");
    }

    public override void DoActionOnActivate(float tickDuration = 1)
    {
        PlayerController playerController = targetObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            //50% 확률로 방향 반전
            playerController.moveFactor *= Random.Range(0, 100) < 50 ? 1.0f : -1.0f;
        }
        else
        {
            Debug.LogWarning("ConfusionDeBuff: PlayerController 컴포넌트를 찾을 수 없습니다.");
        }

        base.DoActionOnActivate(tickDuration);
    }

    public override void RemoveBuffEffect()
    {
        PlayerController playerController = targetObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            //방향 복구
            playerController.moveFactor = Mathf.Abs(playerController.moveFactor);
        }
    }
}
