using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlacksmithAbilityV2 : MonoBehaviour, IAbilityV2
{
    private bool isActivated;

    [SerializeField] private int enchantLevel;
    [SerializeField] private BlacksmithWeaponData curWeaponData;
    [SerializeField] private BlacksmithWeaponData[] weaponDataList;
    [SerializeField] private Animator playerAnimator;

    private PlayerStatus playerStatus;

    private BuffManager playerBuffManager;

    private float soulShard;
    public float SoulShard { get { return soulShard; } set { soulShard = value; } }

    private int upgradeChance;
    public int UpgradeChance { get { return upgradeChance; } set { upgradeChance = value; } }

    private float lastAppliedDamage = 0f;
    private float lastAppliedAdditionalDamage = 0f;
    private float lastAppliedAttackSpeed = 0f;

    public bool IsActivated => isActivated;
    public int EnchantLevel => enchantLevel;

    public UnityEvent onAbilityUpdated;

    public BlacksmithWeaponData CurWeaponData => curWeaponData;

    private List<float> enchantSuccessRates = new()
    {
        1.0f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f
    };

    private Dictionary<int, float> gradeMultipliers = new()
    {
        { 1, 1.0f }, { 2, 0.9f }, { 3, 0.8f }, { 4, 0.1f }
    };

    private Dictionary<int, float> gradeUpgradeChances = new()
    {
        { 1, 0.7f }, { 2, 0.5f }, { 3, 0.3f }
    };

    private const float destroyChance = 0.3f;

    // 현재 사용 중인 무기 데이터의 복사본
    private BlacksmithWeaponData runtimeWeaponData;
    void Awake()
    {
        playerStatus = GetComponent<PlayerStatus>();
        playerBuffManager = GetComponent<BuffManager>();
    }

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            int idx = Random.Range(1, 3);
            SetRuntimeWeaponData(weaponDataList[idx]);
            
            runtimeWeaponData.Rank = 1;
            runtimeWeaponData.EnchantLevel = 0;
            enchantLevel = 0;

            ApplyWeaponBonus();
            ApplyWeaponAnimator();

            InGameUIManager.Instance.ShowStatus($"무기 주조 완료! 현재 무기 : {runtimeWeaponData.WeaponName}");
            onAbilityUpdated.Invoke();
        }
        else
        {
            if (AbilityManager.Instance.blacksmithAbility[5] && !CalcDamage.Instance.IsOnCooldown("blackSmithInvincible"))
            {
                playerBuffManager.ActivateBuff(BuffType.Invincible, 1f);
                CalcDamage.Instance.StartCooldown("blackSmithInvincible", 5f);
            }
            EnchantWeapon();
        }
    }

    public void Deactivate()
    {
        RemoveWeaponBonus();
        Initialize();
        Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
        onAbilityUpdated.Invoke();
    }

    public void ResetAbility()
    {
        RemoveWeaponBonus();
        Initialize();

        onAbilityUpdated?.Invoke();

        Debug.Log("대장장이 스킬: 무기 및 강화 상태 초기화 완료");
    }

    public void EnchantWeapon()
    {
        if (upgradeChance <= 0) 
        {
            InGameUIManager.Instance.ShowStatus("강화 가능 횟수가 부족합니다.");
            return;
        }

        if (curWeaponData.Rank >= 4 && enchantLevel >= 5)
        {
            InGameUIManager.Instance.ShowStatus("최대 강화 단계에 도달했습니다.");
            return;
        }

        upgradeChance--;

        if (enchantLevel >= 5)
        {
            TryUpgradeGrade();
            return;
        }

        float baseProb = GetSuccessRate(enchantLevel);
        float gradeMultiplier = GetGradeMultiplier(curWeaponData.Rank);
        float finalSuccessRate = baseProb * gradeMultiplier;
        if (AbilityManager.Instance.blacksmithAbility[0])
        {
            finalSuccessRate *= 1.3f;
            if (finalSuccessRate >= 1f) finalSuccessRate = 1f;
        }
        float prob = Random.Range(0f, 1f);

        Debug.Log($"[강화 시도] 등급: {GetGradeName(curWeaponData.Rank)}, 단계: {enchantLevel}, 확률: {finalSuccessRate * 100f}% → {prob}");

        if (prob <= finalSuccessRate)
        {
            ++enchantLevel;
            ++curWeaponData.EnchantLevel;
            InGameUIManager.Instance.ShowStatus($"강화 성공! 현재 등급 : {GetGradeName(curWeaponData.Rank)}, 단계 : {enchantLevel}");
            RefreshWeaponBonus(); // 강화 수치 반영
        }
        else
        {
            InGameUIManager.Instance.ShowStatus("강화 실패...");
        }

        onAbilityUpdated.Invoke();
    }

    void TryUpgradeGrade()
    {
        if (curWeaponData.Rank >= 4)
        {
            Debug.Log("이미 S등급입니다.");
            return;
        }

        float chance = gradeUpgradeChances.TryGetValue(curWeaponData.Rank, out var ch) ? ch : 0f;
        float roll = Random.Range(0f, 1f);
        Debug.Log($"[성장 시도] 등급: {GetGradeName(curWeaponData.Rank)}, 확률: {chance * 100f}% → {roll}");

        if (roll <= chance)
        {
            ++curWeaponData.Rank;
            enchantLevel = 0;
            curWeaponData.EnchantLevel = 0;
            RefreshWeaponBonus(); // 성장 반영
            InGameUIManager.Instance.ShowStatus($"장비 성장 완료! 새로운 등급: {GetGradeName(curWeaponData.Rank)}");
        }
        else
        {
            TryDestroyWeapon();
        }

        onAbilityUpdated.Invoke();
    }

    void TryDestroyWeapon()
    {
        float roll = Random.Range(0f, 1f);
        float finalDestroyChance = destroyChance;
        if (AbilityManager.Instance.blacksmithAbility[1]) finalDestroyChance *= 0.5f;
        if (roll <= finalDestroyChance)
        {
            InGameUIManager.Instance.ShowStatus("등급 성장 실패... 장비가 파괴되었습니다.");
            Deactivate();
        }
        else
        {
            InGameUIManager.Instance.ShowStatus("등급 성장 실패... 장비 단계가 초기화되었습니다.");
            curWeaponData.EnchantLevel = 0;
            enchantLevel = 0;
            RefreshWeaponBonus();
        }
    }

    void RefreshWeaponBonus()
    {
        RemoveWeaponBonus();
        ApplyWeaponBonus();

        Debug.Log("추가 데미지 수치 : " + playerStatus.Damage);
        Debug.Log("총 데미지 수치 : " + playerStatus.TotalDamage);
    }

    // 무기 강화를 통한 스테이터스 상승
    void ApplyWeaponBonus()
    {
        int multiplier = 1;
        if (AbilityManager.Instance.blacksmithAbility[4]) multiplier = 2;

        float bonusDamage = curWeaponData.GetEffectiveDamage() * multiplier;
        float bonusAdditionalDamage = curWeaponData.GetEffectiveAdditionalDamage() * multiplier;
        float bonusAttackSpeed = curWeaponData.GetEffectiveAttackSpeed() * multiplier;

        playerStatus.Damage += bonusDamage;
        playerStatus.AdditionalDamage += bonusAdditionalDamage;
        playerStatus.AdditionalAttackSpeed += bonusAttackSpeed;

        lastAppliedDamage = bonusDamage;
        lastAppliedAdditionalDamage = bonusAdditionalDamage;
        lastAppliedAttackSpeed = bonusAttackSpeed;
    }

    // 무기 강화를 통한 스테이터스 상승 초기화
    void RemoveWeaponBonus()
    {
        playerStatus.Damage -= lastAppliedDamage;
        playerStatus.AdditionalDamage -= lastAppliedAdditionalDamage;
        playerStatus.AdditionalAttackSpeed -= lastAppliedAttackSpeed;

        lastAppliedDamage = 0f;
        lastAppliedAdditionalDamage = 0f;
        lastAppliedAttackSpeed = 0f;
    }

    void Start()
    {
        Initialize();
        Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
    }

    void Initialize()
    {
        isActivated = false;

        if (curWeaponData != null)
        {
            curWeaponData.Rank = 1;
            curWeaponData.EnchantLevel = 0;
            curWeaponData = weaponDataList[0];
            ApplyWeaponAnimator(); 
        }

        if (curWeaponData == null)
        {
            enchantLevel = 0;
            curWeaponData = weaponDataList[0];
            ApplyWeaponAnimator();
        }

    }

    void SetRuntimeWeaponData(BlacksmithWeaponData sourceData)
    {
        if (sourceData == null) return;

        // 이전에 생성된 런타임 데이터가 있다면 메모리 누수 방지를 위해 파괴
        if (runtimeWeaponData != null)
        {
            Destroy(runtimeWeaponData);
        }

        // 새 복사본을 만들어 런타임용으로 사용
        runtimeWeaponData = Instantiate(sourceData);
        curWeaponData = runtimeWeaponData; // 인스펙터 확인용으로도 동기화
    }

    float GetSuccessRate(int level)
    {
        return (level < enchantSuccessRates.Count) ? enchantSuccessRates[level] : enchantSuccessRates[^1];
    }

    float GetGradeMultiplier(int rank)
    {
        return gradeMultipliers.TryGetValue(rank, out var mult) ? mult : 1.0f;
    }

    string GetGradeName(int rank)
    {
        return rank switch
        {
            1 => "C",
            2 => "B",
            3 => "A",
            4 => "S",
            _ => "Unknown"
        };
    }

    void ApplyWeaponAnimator()
    {
        if (curWeaponData.AnimatorOverride != null)
        {
            playerAnimator.runtimeAnimatorController = curWeaponData.AnimatorOverride;
        }
    }
}
