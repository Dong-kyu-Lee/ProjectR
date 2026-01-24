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

    public void ShowTooltip(BuffType type, Sprite sprite)
    {
        gameObject.SetActive(true);
        buffNameText.text = type.ToString();
        buffDescriptionText.text = GetBuffDescription(type);
        buffImage.sprite = sprite;

        gameObject.SetActive(true);
        isVisible = true;
    }
    private string GetBuffDescription(BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.AttackDamageIncrease:
                return "공격력이 증가합니다.";
            case BuffType.DamageReductionIncrease:
                return "받는 피해가 감소합니다.";
            case BuffType.Bless:
                return "축복을 받아 다양한 능력이 향상됩니다.";
            case BuffType.Raging:
                return "광분 상태로 공격이 더욱 거칠어집니다.";
            case BuffType.CritDamageIncrease:
                return "치명타 피해량이 증가합니다.";
            case BuffType.CritPercentIncrease:
                return "치명타 확률이 증가합니다.";
            case BuffType.PriceAdditionalIncrease:
                return "탐욕스런 마음이 커져 획득 재화량이 증가합니다.";
            case BuffType.AttackSpeedIncrease:
                return "공격 속도가 증가합니다.";
            case BuffType.MoveSpeedIncrease:
                return "이동 속도가 증가합니다.";
            case BuffType.ExtremeSpeed:
                return "극한의 속도를 얻게 됩니다.";
            case BuffType.EagleEye:
                return "매의 눈처럼 정확도가 증가합니다.";
            case BuffType.BulkUp:
                return "근육이 팽창하며 능력이 상승합니다.";
            case BuffType.IronBody:
                return "단단한 육체로 피해를 견딥니다.";
            case BuffType.Reflection:
                return "일부 피해를 반사합니다.";
            case BuffType.Invincible:
                return "일정 시간 동안 무적 상태가 됩니다.";

            case BuffType.Poison:
                return "중독되어 주기적으로 피해를 입습니다.";
            case BuffType.Burn:
                return "화상으로 인해 주기적으로 피해를 입습니다.";
            case BuffType.Freeze:
                return "빙결되어 이동이 느려지거나 멈춥니다.";
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
                return "무력 스탯이 상승합니다.";
            case BuffType.Force16:
                return "무력 스탯이 크게 상승합니다.";
            case BuffType.Critical4:
                return "치명타 스탯이 소폭 상승합니다.";
            case BuffType.Critical7:
                return "치명타 스탯이 상승합니다.";
            case BuffType.Dexterity7:
                return "재주 스탯이 소폭 상승합니다.";
            case BuffType.Dexterity13:
                return "재주 스탯이 크게 상승합니다.";

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
