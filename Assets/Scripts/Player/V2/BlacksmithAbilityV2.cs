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

    private float lastAppliedDamage = 0f;
    private float lastAppliedAttackSpeed = 0f;

    public bool IsActivated => isActivated;
    public int EnchantLevel => enchantLevel;

    public UnityEvent onAbilityUpdated;

    private List<float> enchantSuccessRates = new()
    {
        1.0f, 0.95f, 0.9f,0.85f, 0.8f, 0.75f, 0.7f, 0.65f, 0.6f, 0.5f
    };

    private Dictionary<int, float> gradeMultipliers = new()
    {
        { 1, 1.0f }, { 2, 0.9f }, { 3, 0.8f }, { 4, 0.7f }
    };

    private Dictionary<int, float> gradeUpgradeChances = new()
    {
        { 1, 0.8f }, { 2, 0.6f }, { 3, 0.4f }
    };

    private const float destroyChance = 0.05f;

    void Awake()
    {
        playerStatus = GetComponent<PlayerStatus>();
    }

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            int idx = Random.Range(1, 3);
            curWeaponData = weaponDataList[idx];
            curWeaponData.Rank = 1;
            curWeaponData.EnchantLevel = 0;
            enchantLevel = 0;
            ApplyWeaponBonus();
            ApplyWeaponAnimator();
            Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
            onAbilityUpdated.Invoke();
        }
    }

    public void Deactivate()
    {
        RemoveWeaponBonus();
        Initialize();
        Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
        onAbilityUpdated.Invoke();
    }

    public void EnchantWeapon()
    {
        if (enchantLevel >= 9)
        {
            TryUpgradeGrade();
            return;
        }

        float baseProb = GetSuccessRate(enchantLevel);
        float gradeMultiplier = GetGradeMultiplier(curWeaponData.Rank);
        float finalSuccessRate = baseProb * gradeMultiplier;
        float prob = Random.Range(0f, 1f);

        Debug.Log($"[강화 시도] 등급: {GetGradeName(curWeaponData.Rank)}, 단계: {enchantLevel}, 확률: {finalSuccessRate * 100f}% → {prob}");

        if (prob <= finalSuccessRate)
        {
            Debug.Log("강화 성공!");
            ++enchantLevel;
            ++curWeaponData.EnchantLevel;
            RefreshWeaponBonus(); // 강화 수치 반영
        }
        else
        {
            Debug.Log("강화 실패...");
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
            Debug.Log($"장비 성장 완료! 새로운 등급: {GetGradeName(curWeaponData.Rank)}");
        }
        else
        {
            Debug.Log("성장 실패...");
            TryDestroyWeapon();
        }

        onAbilityUpdated.Invoke();
    }

    void TryDestroyWeapon()
    {
        float roll = Random.Range(0f, 1f);
        if (roll <= destroyChance)
        {
            Debug.Log("장비 파괴됨! 초기화");
            Deactivate();
        }
        else
        {
            Debug.Log("장비는 살아남았지만 강화 수치는 초기화됨");
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

    // 현재 공격속도는 가산되지 않도록 처리
    void ApplyWeaponBonus()
    {
        float bonusDamage = curWeaponData.GetEffectiveDamage();
        float bonusAttackSpeed = curWeaponData.GetEffectiveAttackSpeed();

        playerStatus.AdditionalDamage += bonusDamage;
        playerStatus.AdditionalAttackSpeed += bonusAttackSpeed;

        lastAppliedDamage = bonusDamage;
        lastAppliedAttackSpeed = bonusAttackSpeed;
    }


    void RemoveWeaponBonus()
    {
        playerStatus.AdditionalDamage -= lastAppliedDamage;
        playerStatus.AdditionalAttackSpeed -= lastAppliedAttackSpeed;

        lastAppliedDamage = 0f;
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
