using UnityEngine;
using UnityEngine.UI;

public class BuffToolTipUI : MonoBehaviour
{
    [SerializeField]
    private Image buffImage;
    [SerializeField]
    private Text buffNameText;
    [SerializeField]
    private Text buffDescriptionText;

    private bool isVisible = false;
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        isVisible = false;
    }

    public void ShowTooltip(Buff buff, Sprite sprite)
    {
        gameObject.SetActive(true);
        buffNameText.text = buff.BuffType.ToString();
        buffDescriptionText.text = GetBuffDescription(buff);
        buffImage.sprite = sprite;

        gameObject.SetActive(true);
        isVisible = true;
    }
    private string GetBuffDescription(Buff buff)
    {
        switch (buff.BuffType)
        {
            case BuffType.AttackDamageIncrease:
                return $"추가 피해량 +{buff.currentBuffValue["atkDmgInc"] * 100}%";
            case BuffType.DamageReductionIncrease:
                return $"피해량 감소 +{buff.currentBuffValue["damageReduceInc"] * 100}%";
            case BuffType.Bless:
                return $"{buff.BuffEffectTick}초당 체력 +{buff.currentBuffValue["healAmount"]} 회복";
            case BuffType.Raging:
                return $"추가 피해량 +{buff.currentBuffValue["atkDmgInc"] * 100}%, 치명타 확률 +{buff.currentBuffValue["critPerInc"] * 100}%, " +
                       $"치명타 피해량 +{buff.currentBuffValue["critDmgInc"] * 100}%, 피해 감소량 {buff.currentBuffValue["damageReduceInc"] * 100}%";
            case BuffType.CritDamageIncrease:
                return $"치명타 피해량 +{buff.currentBuffValue["critDmgInc"] * 100}%";
            case BuffType.CritPercentIncrease:
                return $"치명타 확률 +{buff.currentBuffValue["critPerInc"] * 100}%";
            case BuffType.PriceAdditionalIncrease:
                return $"재화 획득량 +{buff.currentBuffValue["priceInc"] * 100}%";
            case BuffType.AttackSpeedIncrease:
                return $"공격 속도 +{buff.currentBuffValue["atkSpdInc"] * 100}%";
            case BuffType.MoveSpeedIncrease:
                return $"이동 속도 +{buff.currentBuffValue["moveSpdInc"] * 100}%";
            case BuffType.ExtremeSpeed:
                return $"공격 속도 +{buff.currentBuffValue["atkSpdInc"] * 100}%, 이동 속도 +{buff.currentBuffValue["moveSpdInc"] * 100}%";
            case BuffType.EagleEye:
                return $"치명타 확률 +{buff.currentBuffValue["critPerInc"] * 100}%, 치명타 피해량 +{buff.currentBuffValue["critDmgInc"] * 100}%";
            case BuffType.BulkUp:
                return $"추가 피해량 +{buff.currentBuffValue["atkDmgInc"] * 100}%, 피해 감소량 +{buff.currentBuffValue["damageReduceInc"] * 100}%";
            case BuffType.IronBody:
                return $"추가 피해량 +{buff.currentBuffValue["atkDmgInc"] * 100}%, 공격 속도 {buff.currentBuffValue["atkSpdInc"] * 100}%, " +
                       $"이동 속도 {buff.currentBuffValue["moveSpdInc"] * 100}%";
            case BuffType.Destruction:
                return $"피해량 +{buff.currentBuffValue["atkDmgInc"]}";
            case BuffType.Reflection:
                return "받는 모든 피해 반사";
            case BuffType.Invincible:
                return "받는 모든 피해 면역";

            case BuffType.Poison:
                return "중독되어 주기적으로 피해를 입습니다.";
            case BuffType.Burn:
                return "화상으로 인해 주기적으로 피해를 입습니다.";
            case BuffType.Freeze:
                return "빙결되어 이동이 느려지거나 멈춥니다.";
            case BuffType.Curse:
                return "저주에 걸려 받는 피해가 증가합니다.";
            case BuffType.Slow:
                return "이동 속도가 감소합니다.";
            case BuffType.Sleep:
                return "수면 상태로 행동이 제한됩니다.";
            case BuffType.Buzzed:
                return "약간 취해 움직임이 이상해집니다.";
            case BuffType.Drunken:
                return "만취 상태로 행동이 둔해집니다.";
            case BuffType.Bleeding:
                return "출혈로 인해 지속적으로 피해를 입습니다.";
            case BuffType.Stun:
                return "기절하여 행동이 불가능합니다.";
            case BuffType.StoneCurse:
                return "석화 상태가 되어 움직일 수 없습니다.";
            case BuffType.Confusion:
                return "혼란에 빠져 방향이 바뀔 수 있습니다.";

            case BuffType.Force7:
                return $"피해량 +{buff.currentBuffValue["atkDmgInc"]}";
            case BuffType.Force16:
                return $"추가 피해량 +{buff.currentBuffValue["atkDmgInc"] * 100}%";
            case BuffType.Critical4:
                return $"이동 속도 +{buff.currentBuffValue["atkDmgInc"] * 100}%";
            case BuffType.Critical7:
                return $"치명타 확률 +{buff.currentBuffValue["critPerInc"] * 100}%";
            case BuffType.Dexterity7:
                return $"피해량 +{buff.currentBuffValue["atkDmgInc"]}";
            case BuffType.Dexterity13:
                return $"공격 속도 +{buff.currentBuffValue["atkSpdInc"] * 100}%";

            default:
                return "설명 없음";
        }

    }
    public void Hide()
    {
        gameObject.SetActive(false);
        isVisible = false;
    }
    public bool IsVisible()
    {
        return isVisible;
    }
}
