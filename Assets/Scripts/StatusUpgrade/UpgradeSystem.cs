using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    private HashSet<string> unlockedEffects = new HashSet<string>();  // 중복 실행 방지용.

    private PlayerStatus playerStatus;
    private UpgradeStatus upgradeStatus;
    private StatusEffect statusEffect;
    private StatusValueText statusValueText;

    private void Start()
    {
        ResetPlayerInfo();
        statusEffect = GetComponent<StatusEffect>();
        statusValueText = transform.GetComponent<StatusValueText>();
    }

    void OnEnable()
    {
        // 리스너 등록
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnPlayerCharacterChanged.AddListener(ResetPlayerInfo);
    }

    void OnDisable()
    {
        // 리스너 해제 (메모리 누수 방지)
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnPlayerCharacterChanged.RemoveListener(ResetPlayerInfo);
    }

    public void ResetPlayerInfo()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.CurrentPlayer != null)
        {
            playerStatus = PlayerManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
            upgradeStatus = PlayerManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
            CheckUnlockAll();
            Debug.Log("캐릭터 정보 초기화");
        }
    }

    // 스킬 포인트 사용, 업그레이드 스테이터스 증가.
    public void IncreaseStat(string statName)
    {
        if (upgradeStatus == null) return;

        if (upgradeStatus.StatPoint <= 0)
        {
            if (InGameUIManager.Instance != null)
            {
                InGameUIManager.Instance.ShowStatus($"스탯포인트가 부족합니다.");
            }
            return;
        }

        switch (statName)
        {
            case "force":
                upgradeStatus.Force++;
                playerStatus.Damage++;
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
                playerStatus.BuffDuration += 0.02f;
                CheckUnlock("mystery", upgradeStatus.Mystery);
                break;
            case "curse":
                upgradeStatus.Curse++;
                playerStatus.DebuffDamage += 0.03f;
                CheckUnlock("curse", upgradeStatus.Curse);
                break;
            default:
                Debug.Log("잘못된 스테이터스 이름");
                return;
        }

        upgradeStatus.StatPoint--;
        
        if (statusValueText != null)
            statusValueText.SetupValueText(upgradeStatus);

        // 스탯 증가 후 인벤토리 실시간 동기화
        SyncCharacterInfoUI();
    }

    // 스테이터스 초기화.
    public void ResetStat()
    {
        if (playerStatus == null || upgradeStatus == null) return;

        playerStatus.Damage -= upgradeStatus.Force * 1;
        playerStatus.AdditionalDamageReduction -= upgradeStatus.Indurance * 0.01f;
        playerStatus.CriticalPercent -= upgradeStatus.Critical * 0.02f;
        playerStatus.AdditionalAttackSpeed -= upgradeStatus.Dexterity * 0.02f;
        playerStatus.BuffDuration -= upgradeStatus.Mystery * 0.02f;
        playerStatus.DebuffDamage -= upgradeStatus.Curse * 0.03f;

        upgradeStatus.Force = upgradeStatus.Indurance = upgradeStatus.Critical = upgradeStatus.Dexterity = upgradeStatus.Mystery = upgradeStatus.Curse = 0;
        upgradeStatus.StatPoint = 0;
        CheckUnlockAll();
        
        if (statusValueText != null)
            statusValueText.SetupValueText(upgradeStatus);

        // 스탯 초기화 후 인벤토리 실시간 동기화
        SyncCharacterInfoUI();
    }

    // 인벤토리가 열려있는지 확인하고 실시간으로 UI를 새로고침 해주는 함수
    private void SyncCharacterInfoUI()
    {
        if (InGameUIManager.Instance != null && InGameUIManager.Instance.characterInfoUI != null)
        {
            // 인벤토리(CharacterInfo)가 활성화되어 있을 때만 갱신
            if (InGameUIManager.Instance.characterInfoUI.gameObject.activeInHierarchy)
            {
                InGameUIManager.Instance.characterInfoUI.RefreshStatusUI();
            }
        }
        else
        {
            // Inspector 누락 등을 대비한 안전장치
            CharacterInfo charInfo = FindObjectOfType<CharacterInfo>();
            if (charInfo != null && charInfo.gameObject.activeInHierarchy)
            {
                charInfo.RefreshStatusUI();
            }
        }
    }

    // 모든 특수 효과 해금 여부 확인
    private void CheckUnlockAll()
    {
        CheckUnlock("force", upgradeStatus.Force);
        CheckUnlock("indurance", upgradeStatus.Indurance);
        CheckUnlock("critical", upgradeStatus.Critical);
        CheckUnlock("dexterity", upgradeStatus.Dexterity);
        CheckUnlock("mystery", upgradeStatus.Mystery);
        CheckUnlock("curse", upgradeStatus.Curse);
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

    // 스탯 증가 온클릭.
    public void IncreaseStatOnClick(string statName)
    {
        IncreaseStat(statName);
    }
}