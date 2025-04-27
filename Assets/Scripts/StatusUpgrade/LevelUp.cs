using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private UpgradeStatus upgradeStatus;
    private UpgradeSystem upgradeSystem;
    private StatusValueText statusValueText;

    // 요구 경험치 ex) 레벨 1 -> 레벨 2 요구 경험치 = requiredExp[1].
    public static readonly int[] requiredExp;

    private void Start()
    {
        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
        upgradeSystem = transform.GetComponentInChildren<UpgradeSystem>();
        statusValueText = transform.GetComponentInChildren<StatusValueText>();
    }

    // 경험치 통 정의.
    static LevelUp()
    {
        int maxLevel = 50; // 최대 레벨.
        double coefficient = 20; // 계수.
        requiredExp = new int[maxLevel + 1];

        for (int i = 1; i <= maxLevel; i++)
        {
            double exp = Math.Pow((i * 100.0 / 99), 2) * coefficient;
            requiredExp[i] = (int)Math.Round(exp);
        }
    }

    // 경험치 증가.
    public void IncreaseExp(float value)
    {
        playerStatus.Exp += value;
        if (playerStatus.Exp > requiredExp[(int)playerStatus.Level])
        {
            UpLevel();
        }
    }

    // 레벨 업.
    public void UpLevel()
    {
        playerStatus.Level++;
        upgradeStatus.StatPoint++;

        playerStatus.Damage += 1;
        playerStatus.MaxHp += 5;
        playerStatus.Hp += 5;

        playerStatus.Exp -= requiredExp[(int)playerStatus.Level - 1];

        if (playerStatus.Exp > requiredExp[(int)playerStatus.Level])
        {
            UpLevel();
        }

        statusValueText.SetupValueText(upgradeStatus);
    }

    // 레벨 초기화.
    public void ResetLevel()
    {
        upgradeSystem.ResetStat();
        playerStatus.Damage -= playerStatus.Level - 1;
        playerStatus.MaxHp -= (playerStatus.Level - 1) * 5;
        playerStatus.Level = 1;
        playerStatus.Exp = 0;
    }
}
