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
        StartCoroutine(InitPlayerStatus());
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

    private IEnumerator InitPlayerStatus()
    {
        // 플레이어가 확실히 준비될 때까지 대기
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null);

        statusValueText = transform.GetComponentInChildren<StatusValueText>(true);
        upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
        levelup = transform.GetComponent<LevelUp>();
    }
}