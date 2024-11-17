using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOnClick : MonoBehaviour
{
    [SerializeField] private UpgradeStatus upgradeStatus;
    [SerializeField] private StatusValueText statusValueText;

    public void IncreaseStatOnClick(string statName)
    {
        upgradeStatus.IncreaseStat(statName);
    }

    public void IncreaseSPOnClick()
    {
        upgradeStatus.SkillPoint++;
        statusValueText.skillpointText.text = "스킬포인트 : " + upgradeStatus.SkillPoint;
    }

    public void ResetStatOnClick()
    {
        upgradeStatus.ResetStat();
    }
}
