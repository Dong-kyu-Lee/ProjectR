using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerObj;

public class UpgradeOnClick : MonoBehaviour
{
    [SerializeField] private UpgradeSystem upgradeSystem;
    [SerializeField] private UpgradeStatus upgradeStatus;
    [SerializeField] private StatusValueText statusValueText;
    [SerializeField] private LevelUp levelup;

    private void Start()
    {
        //upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
    }
    
    // 스탯 증가 온클릭.
    public void IncreaseStatOnClick(string statName)
    {
        upgradeSystem.IncreaseStat(statName);
    }

    // 경험치 증가 온클릭.
    public void IncreaseExpOnClick(int value)
    {
        levelup.IncreaseExp(value);
    }

    // 스탯포인트 증가 온클릭.
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
