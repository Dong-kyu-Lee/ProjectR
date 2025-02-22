using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionBuff : Buff
{
    public ReflectionBuff(float totalDuration, GameObject target) : base(totalDuration, target) { }
    public override void ApplyBuffEffect()
    {
        Debug.Log("반사 버프 활성화");

        //반사버프의 데미지 처리 로직은 PlayerController의 Hit() 따위의 메서드에서 처리해야할 듯 싶음.
        //대충 ActiveBuffDict.ContainsKey(BuffType.Reflection) 이런 식으로 체크해서 버프가 true면 데미지 처리하도록 하면 될 듯.
    }

    public override void RemoveBuffEffect()
    {
        Debug.Log("반사 버프 비활성화");
    }
}
