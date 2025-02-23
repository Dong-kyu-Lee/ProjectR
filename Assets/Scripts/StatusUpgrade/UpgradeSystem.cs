using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    private HashSet<string> unlockedEffects = new HashSet<string>();  // 중복 실행 방지용.

    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private UpgradeStatus upgradeStatus;
    [SerializeField] private StatusEffect statusEffect;
    private StatusValueText statusValueText;

    private void Awake()
    {
        //playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        //upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
        statusValueText = gameObject.GetComponent<StatusValueText>();
    }

    // 스킬 포인트 사용, 업그레이드 스테이터스 증가.
    public void IncreaseStat(string statName)
    {
        if (upgradeStatus.StatPoint <= 0)
        {
            Debug.Log("스킬포인트가 없습니다.");
            return;
        }

        switch (statName)
        {
            case "force":
                upgradeStatus.Force++;
                playerStatus.Damage += 2;
                CheckUnlock("force", upgradeStatus.Force);
                break;
            case "indurance":
                playerStatus.AdditionalDamageReduction -= upgradeStatus.Indurance * 0.01f;
                upgradeStatus.Indurance++;
                playerStatus.AdditionalDamageReduction += upgradeStatus.Indurance * 0.01f;
                CheckUnlock("indurance", upgradeStatus.Indurance);
                break;
            case "critical":
                upgradeStatus.Critical++;
                playerStatus.CriticalPercent += 0.02f;
                CheckUnlock("critical", upgradeStatus.Critical);
                break;
            case "dexterity":
                upgradeStatus.Dexterity++;
                playerStatus.AdditionalAttackSpeed += 0.02f;
                CheckUnlock("dexterity", upgradeStatus.Dexterity);
                break;
            case "mystery":
                upgradeStatus.Mystery++;
                CheckUnlock("mystery", upgradeStatus.Mystery);
                break;
            default:
                Debug.Log("잘못된 스테이터스 이름");
                return;
        }

        upgradeStatus.StatPoint--;
        statusValueText.SetupValueText(upgradeStatus);
    }

    // 스테이터스 초기화.
    public void ResetStat()
    {
        playerStatus.Damage -= upgradeStatus.Force * 2;
        playerStatus.AdditionalDamageReduction -= upgradeStatus.Indurance;
        playerStatus.CriticalPercent -= upgradeStatus.Critical * 0.02f;
        playerStatus.AdditionalAttackSpeed -= upgradeStatus.Dexterity * 0.02f;

        upgradeStatus.Force = upgradeStatus.Indurance = upgradeStatus.Critical = upgradeStatus.Dexterity = upgradeStatus.Mystery = 0;
        upgradeStatus.StatPoint = 0;
        CheckUnlock("force", upgradeStatus.Force);
        CheckUnlock("indurance", upgradeStatus.Indurance);
        CheckUnlock("critical", upgradeStatus.Critical);
        CheckUnlock("dexterity", upgradeStatus.Dexterity);
        CheckUnlock("mystery", upgradeStatus.Mystery);
        statusValueText.SetupValueText(upgradeStatus);
    }

    // 특수 효과 해금 여부 확인.
    private void CheckUnlock(string statName, int statValue)
    {
        int[] unlockPoints = { 1, 4, 7, 10, 13, 16 };

        foreach (int point in unlockPoints)
        {
            if (statValue >= point && !unlockedEffects.Contains($"{statName}_{point}"))
            {
                statusEffect.EnableEffect(statName, point);
                unlockedEffects.Add($"{statName}_{point}");
            }

            if (statValue < point && unlockedEffects.Contains($"{statName}_{point}"))
            {
                statusEffect.DisableEffect(statName, point);
                unlockedEffects.Remove($"{statName}_{point}");
            }
        }
    }
}
