using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuffInfo", menuName = "Buff/BuffInfo")]
public class BuffInfo : ScriptableObject
{
    public BuffType buffType;    // 해당 버프의 열거형 타입
    public Sprite buffIcon;      // 버프 아이콘 이미지
    public string buffName;      // 버프 이름
    [TextArea]
    public string description;   // 버프 설명

    public bool isDebuff;
}
