using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct SkillInfo
{
    public string skillName;
    public string skillDesc;
    public string skillCool;
}

public class SkillToolTipUI : MonoBehaviour
{
    public Text skillNameText;
    public Text skillDescText;
    public Text skillCoolText;

    [SerializeField] private SkillInfo bartenderSkillInfo = new SkillInfo();
    [SerializeField] private SkillInfo blacksmithSkillInfo = new SkillInfo();

    // 스킬 설명 툴팁 내용
    public void OnEnable()
    {
        bartenderSkillInfo.skillName = "바텐더 스킬(Q) : 술 제조";
        bartenderSkillInfo.skillDesc = $"특별한 술을 제조하여 다음 {(AbilityManager.Instance.bartenderAbility[4] ? 20 : 10)}번의 술병 투척에 발화, 중독, 빙결 중 하나의 디버프를 추가한다.";
        int coolTime = Mathf.RoundToInt(AbilityManager.Instance.bartenderAbility[5] ? 12f : 20f);
        bartenderSkillInfo.skillCool = $"쿨타임 {coolTime}초";

        blacksmithSkillInfo.skillName = "대장장이 스킬(Q) : 무기 제작 / 강화";
        blacksmithSkillInfo.skillDesc = $"대장장이 고유의 무기를 제작하고 강화하여 더 강한 공격을 할 수 있게 한다.";
        blacksmithSkillInfo.skillCool = $"쿨타임 없음";
    }

    // 스킬 설명 툴팁 확인
    public void ShowTooltip(string character)
    {
        gameObject.SetActive(true);

        switch (character)
        {
            case "bartender":
                skillNameText.text = bartenderSkillInfo.skillName;
                skillDescText.text = bartenderSkillInfo.skillDesc;
                skillCoolText.text = bartenderSkillInfo.skillCool;
                break;
            case "blacksmith":
                skillNameText.text = blacksmithSkillInfo.skillName;
                skillDescText.text = blacksmithSkillInfo.skillDesc;
                skillCoolText.text = blacksmithSkillInfo.skillCool;
                break;
        }
    }

    // 스킬 설명 툴팁 숨기기
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
