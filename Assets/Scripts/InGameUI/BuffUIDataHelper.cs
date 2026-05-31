using System;
using System.Collections.Generic;

// UI에 필요한 버프 관련 데이터를 가공하고 제공하는 정적 헬퍼 클래스
public static class BuffUIDataHelper
{
    // BuffType을 키(Key)로, 설명을 만들어내는 함수(Func)를 값(Value)으로 가지는 딕셔너리
    private static readonly Dictionary<BuffType, Func<Buff, string>> buffDescriptionMap = new Dictionary<BuffType, Func<Buff, string>>()
    {
        { BuffType.AttackDamageIncrease, b => $"추가 피해량 +{b.currentBuffValue["atkDmgInc"] * 100}%" },
        { BuffType.DamageReductionIncrease, b => $"피해량 감소 +{b.currentBuffValue["damageReduceInc"] * 100}%" },
        { BuffType.Bless, b => $"{b.BuffEffectTick}초당 체력 +{b.currentBuffValue["healAmount"]} 회복" },
        { BuffType.Raging, b => $"추가 피해량 +{b.currentBuffValue["atkDmgInc"] * 100}%, 치명타 확률 +{b.currentBuffValue["critPerInc"] * 100}%, 치명타 피해량 +{b.currentBuffValue["critDmgInc"] * 100}%, 피해 감소량 {b.currentBuffValue["damageReduceInc"] * 100}%" },
        { BuffType.CritDamageIncrease, b => $"치명타 피해량 +{b.currentBuffValue["critDmgInc"] * 100}%" },
        { BuffType.CritPercentIncrease, b => $"치명타 확률 +{b.currentBuffValue["critPerInc"] * 100}%" },
        { BuffType.PriceAdditionalIncrease, b => $"재화 획득량 +{b.currentBuffValue["priceInc"] * 100}%" },
        { BuffType.AttackSpeedIncrease, b => $"공격 속도 +{b.currentBuffValue["atkSpdInc"] * 100}%" },
        { BuffType.MoveSpeedIncrease, b => $"이동 속도 +{b.currentBuffValue["moveSpdInc"] * 100}%" },
        { BuffType.ExtremeSpeed, b => $"공격 속도 +{b.currentBuffValue["atkSpdInc"] * 100}%, 이동 속도 +{b.currentBuffValue["moveSpdInc"] * 100}%" },
        { BuffType.EagleEye, b => $"치명타 확률 +{b.currentBuffValue["critPerInc"] * 100}%, 치명타 피해량 +{b.currentBuffValue["critDmgInc"] * 100}%" },
        { BuffType.BulkUp, b => $"추가 피해량 +{b.currentBuffValue["atkDmgInc"] * 100}%, 피해 감소량 +{b.currentBuffValue["damageReduceInc"] * 100}%" },
        { BuffType.IronBody, b => $"추가 피해량 +{b.currentBuffValue["atkDmgInc"] * 100}%, 공격 속도 {b.currentBuffValue["atkSpdInc"] * 100}%, 이동 속도 {b.currentBuffValue["moveSpdInc"] * 100}%" },
        { BuffType.Destruction, b => $"피해량 +{b.currentBuffValue["atkDmgInc"]}" },
        { BuffType.Reflection, b => "받는 모든 피해 반사" },
        { BuffType.Invincible, b => "받는 모든 피해 면역" },

        { BuffType.Poison, b => "중독되어 주기적으로 피해를 입습니다." },
        { BuffType.Burn, b => "화상으로 인해 주기적으로 피해를 입습니다." },
        { BuffType.Freeze, b => "빙결되어 이동이 느려지거나 멈춥니다." },
        { BuffType.Curse, b => "저주에 걸려 받는 피해가 증가합니다." },
        { BuffType.Slow, b => "이동 속도가 감소합니다." },
        { BuffType.Sleep, b => "수면 상태로 행동이 제한됩니다." },
        { BuffType.Buzzed, b => "약간 취해 움직임이 이상해집니다." },
        { BuffType.Drunken, b => "만취 상태로 행동이 둔해집니다." },
        { BuffType.Bleeding, b => "출혈로 인해 지속적으로 피해를 입습니다." },
        { BuffType.Stun, b => "기절하여 행동이 불가능합니다." },
        { BuffType.StoneCurse, b => "석화 상태가 되어 움직일 수 없습니다." },
        { BuffType.Confusion, b => "혼란에 빠져 방향이 바뀔 수 있습니다." },

        { BuffType.Force7, b => $"피해량 +{b.currentBuffValue["atkDmgInc"]}" },
        { BuffType.Force16, b => $"추가 피해량 +{b.currentBuffValue["atkDmgInc"] * 100}%" },
        { BuffType.Critical4, b => $"이동 속도 +{b.currentBuffValue["atkDmgInc"] * 100}%" }, // 원본 코드 유지 (이동속도인데 atkDmgInc를 쓰고 있음, 확인 필요)
        { BuffType.Critical7, b => $"치명타 확률 +{b.currentBuffValue["critPerInc"] * 100}%" },
        { BuffType.Dexterity7, b => $"피해량 +{b.currentBuffValue["atkDmgInc"]}" },
        { BuffType.Dexterity13, b => $"공격 속도 +{b.currentBuffValue["atkSpdInc"] * 100}%" }
    };

    // 버프 설명을 반환하는 함수
    public static string GetDescription(Buff buff)
    {
        if (buff == null) return "버프 정보 없음";

        if (buffDescriptionMap.TryGetValue(buff.BuffType, out var descFunc))
        {
            return descFunc(buff); // 해당 버프에 맞는 포맷팅 람다 함수 실행
        }

        return "설명 없음";
    }

    // 포션류 버프인지 판별하는 함수
    public static bool IsPotionBuff(BuffType type)
    {
        // 최적화: 추후 ToString() 대신 (int)type 범위 체크나 
        // Potion 여부를 나타내는 별도 플래그/배열을 사용하도록 이 안에서만 수정하면 됨
        return type.ToString().StartsWith("Potion_");
    }
}