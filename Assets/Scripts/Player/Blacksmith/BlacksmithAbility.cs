using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlacksmithAbility : MonoBehaviour, IAbility
{
    bool isActivated;

    [SerializeField]
    int enchantLevel;

    [SerializeField]
    BlacksmithWeaponData curWeaponData;

    [SerializeField]
    BlacksmithWeaponData[] weaponDataList;

    public bool IsActivated { get { return isActivated; } }
    public int EnchantLevel { get { return enchantLevel; } }

    public UnityEvent onAbilityUpdated;

    List<float> enchantSuccessRates = new List<float> {
        1.0f, 0.95f, 0.9f, 0.85f, 0.8f, 0.75f, 0.7f, 0.65f, 0.6f, 0.5f
    };

    Dictionary<int, float> gradeMultipliers = new Dictionary<int, float>
    {
        {1, 1.0f}, {2, 0.9f}, {3, 0.8f}, {4, 0.7f}
    };

    Dictionary<int, float> gradeUpgradeChances = new Dictionary<int, float>
    {
        {1, 0.8f},
        {2, 0.6f},
        {3, 0.4f}
    };

    const float destroyChance = 0.05f;

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            curWeaponData = weaponDataList[1];
            Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
            onAbilityUpdated.Invoke();
        }
    }

    public void Deactivate()
    {
        Initialize();
        curWeaponData = weaponDataList[0];
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

        Debug.Log($"[강화 시도] 현재 등급: {GetGradeName(curWeaponData.Rank)}, 강화 단계: {enchantLevel}, 최종 확률: {finalSuccessRate * 100f}%, 뽑은 값: {prob}");

        if (prob <= finalSuccessRate)
        {
            Debug.Log("강화 성공!");
            ++enchantLevel;
            if (enchantLevel >= 9)
            {
                Debug.Log("9강 달성! 장비 성장 가능!");
            }
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
            Debug.Log("이미 S등급입니다. 더 이상 성장할 수 없습니다.");
            return;
        }

        float upgradeChance = gradeUpgradeChances.ContainsKey(curWeaponData.Rank) ? gradeUpgradeChances[curWeaponData.Rank] : 0f;
        float roll = Random.Range(0f, 1f);

        Debug.Log($"[성장 시도] 현재 등급: {GetGradeName(curWeaponData.Rank)}, 성공 확률: {upgradeChance * 100f}%, 뽑은 값: {roll}");

        if (roll <= upgradeChance)
        {
            ++curWeaponData.Rank;
            enchantLevel = 0;
            Debug.Log($"장비가 성장했습니다! 새로운 등급: {GetGradeName(curWeaponData.Rank)}");
        }
        else
        {
            Debug.Log("장비 성장 실패...");
            TryDestroyWeapon();
        }

        onAbilityUpdated.Invoke();
    }

    void TryDestroyWeapon()
    {
        float destroyRoll = Random.Range(0f, 1f);

        if (destroyRoll <= destroyChance)
        {
            Debug.Log("장비가 파괴되었습니다! 초기화됩니다.");
            Deactivate();
        }
        else
        {
            Debug.Log("장비는 파괴되지 않았습니다. 강화 레벨만 초기화됩니다.");
            enchantLevel = 0;
        }
    }

    void Start()
    {
        Initialize();
        Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
    }

    void Update()
    {

    }

    void Initialize()
    {
        isActivated = false;
        enchantLevel = 0;
    }

    float GetSuccessRate(int level)
    {
        if (level < enchantSuccessRates.Count)
        {
            return enchantSuccessRates[level];
        }
        else
        {
            return enchantSuccessRates[enchantSuccessRates.Count - 1];
        }
    }

    float GetGradeMultiplier(int rank)
    {
        if (gradeMultipliers.ContainsKey(rank))
        {
            return gradeMultipliers[rank];
        }
        else
        {
            return 1.0f;
        }
    }

    string GetGradeName(int rank)
    {
        switch (rank)
        {
            case 1: return "C";
            case 2: return "B";
            case 3: return "A";
            case 4: return "S";
            default: return "Unknown";
        }
    }
}
