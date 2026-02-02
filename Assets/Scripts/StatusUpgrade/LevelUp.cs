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
    [SerializeField] private int maxlevel = 50;

    public static LevelUp Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
            return;
        }
    }

    private void Start()
    {
        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
        upgradeSystem = transform.GetComponentInChildren<UpgradeSystem>(true);
        statusValueText = transform.GetComponentInChildren<StatusValueText>(true);
        //UI갱신용
        if (InGameUIManager.Instance != null && playerStatus != null)
        {
            int maxExp = requiredExp[(int)playerStatus.Level];
            InGameUIManager.Instance.UpdateExpUI(playerStatus.Exp, maxExp);
            InGameUIManager.Instance.UpdateLevelUI((int)playerStatus.Level);
        }
    }

    // 경험치 통 정의.
    static LevelUp()
    {
        int maxLevel = 50; // 최대 레벨.
        requiredExp = new int[maxLevel + 1];

        for (int i = 1; i < maxLevel; i++)
        {
            double exp = 100 + 2 * Math.Pow(i, 2) * Math.Pow(1.03, i);
            requiredExp[i] = (int)Math.Round(exp);
        }
        requiredExp[maxLevel] = 0;
    }

    // 경험치 증가.
    public void IncreaseExp(float value)
    {
        if (playerStatus == null)
        {
            playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
            if (playerStatus == null) return;
        }
        if (playerStatus.Level < maxlevel)
        {
            playerStatus.Exp += value;

            if (InGameUIManager.Instance != null)
            {
                int maxExp = requiredExp[(int)playerStatus.Level];
                InGameUIManager.Instance.UpdateExpUI(playerStatus.Exp, maxExp);
            }
            else
            {
                Debug.Log("InGameUIManager가 할당되지 않음");
            }

            if (playerStatus.Exp > requiredExp[(int)playerStatus.Level])
            {
                UpLevel();
            }
        }
        else
        {
            playerStatus.Exp = 0;
            if (InGameUIManager.Instance != null)
            {
                InGameUIManager.Instance.UpdateExpUI(0, 1);
            }
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

        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.UpdateLevelUI((int)playerStatus.Level);
            InGameUIManager.Instance.UpdateHpSmooth(playerStatus.Hp, playerStatus.MaxHp);
        }

        if (playerStatus.Level == maxlevel) playerStatus.Exp = 0;
        if (playerStatus.Exp > requiredExp[(int)playerStatus.Level] && playerStatus.Level < maxlevel)
        {
            UpLevel();
        }

        statusValueText.SetupValueText(upgradeStatus);

        if (InGameUIManager.Instance != null && playerStatus.Level < maxlevel)
        {
            int maxExp = requiredExp[(int)playerStatus.Level];
            InGameUIManager.Instance.UpdateExpUI(playerStatus.Exp, maxExp);
        }
    }

    // 레벨 초기화.
    public void ResetLevel()
    {
        upgradeSystem.ResetStat();
        playerStatus.Damage -= playerStatus.Level - 1;
        playerStatus.MaxHp -= (playerStatus.Level - 1) * 5;
        playerStatus.Level = 1;
        playerStatus.Exp = 0;

        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.UpdateLevelUI(1);
            InGameUIManager.Instance.UpdateExpUI(0, requiredExp[1]);
            InGameUIManager.Instance.UpdateHpSmooth(playerStatus.Hp, playerStatus.MaxHp);
        }
    }
}
