using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOnClick : MonoBehaviour
{
    [SerializeField] private UpgradeStatus upgradeStatus;
    [SerializeField] private StatusValueText statusValueText;
    [SerializeField] private LevelUp levelup;

    public void IncreaseStatOnClick(string statName)
    {
        upgradeStatus.IncreaseStat(statName);
    }

    public void IncreaseExpOnClick(int value)
    {
        levelup.IncreaseExp(value);
    }

    public void IncreaseSPOnClick()
    {
        upgradeStatus.SkillPoint++;
        statusValueText.skillpointText.text = "스킬포인트 : " + upgradeStatus.SkillPoint;
    }

    public void ResetLevelOnClick()
    {
        levelup.ResetLevel();
    }
}
