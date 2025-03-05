using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UpgradeOnClick : MonoBehaviour
{
    private StatusValueText statusValueText;
    private UpgradeStatus upgradeStatus;
    private LevelUp levelup;

    private void Start()
    {
        statusValueText = transform.GetComponentInChildren<StatusValueText>();
        upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
        levelup = gameObject.GetComponent<LevelUp>();
    }

    // 경험치 증가 온클릭.
    public void IncreaseExpOnClick(int value)
    {
        levelup.IncreaseExp(value);
    }

    // 스탯포인트 증가 온클릭.
    public void IncreaseSPOnClick()
    {
        upgradeStatus.StatPoint += 10;
        statusValueText.statpointText.text = "스탯포인트 : " + upgradeStatus.StatPoint;
    }

    public void ResetLevelOnClick()
    {
        levelup.ResetLevel();
    }
}
