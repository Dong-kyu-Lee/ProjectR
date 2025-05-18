using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 대장장이 캐릭터 고유 능력 (장비 제작 및 강화)
public class BlacksmithAbilityV2 : MonoBehaviour, IAbilityV2
{
    private bool isActivated;

    [SerializeField]
    private int enchantLevel;

    [SerializeField]
    private BlacksmithWeaponData curWeaponData;

    [SerializeField]
    private BlacksmithWeaponData[] weaponDataList;

    public bool IsActivated => isActivated;
    public int EnchantLevel => enchantLevel;

    public UnityEvent onAbilityUpdated;

    private List<float> enchantSuccessRates = new List<float> {
        1.0f, 0.95f, 0.9f, 0.85f, 0.8f, 0.75f, 0.7f, 0.65f, 0.6f, 0.5f
    };

    private Dictionary<int, float> gradeMultipliers = new Dictionary<int, float> {
        {1, 1.0f}, {2, 0.9f}, {3, 0.8f}, {4, 0.7f}
    };

    private Dictionary<int, float> gradeUpgradeChances = new Dictionary<int, float> {
        {1, 0.8f}, {2, 0.6f}, {3, 0.4f}
    };

    private const float destroyChance = 0.05f;

    // 고유 능력 발동: 무기 초기화
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
        Debug.Log($"[강화 시도] 등급: {GetGradeName(curWeaponData.Rank)}, 단계: {enchantLevel}, 확률: {finalSuccessRate * 100f}% → {prob}");

        if (prob <= finalSuccessRate)
        {
            Debug.Log("강화 성공!");
            ++enchantLevel;
            if (enchantLevel >= 9)
                Debug.Log("9강 달성! 장비 성장 가능!");
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

        float chance = gradeUpgradeChances.ContainsKey(curWeaponData.Rank)
            ? gradeUpgradeChances[curWeaponData.Rank] : 0f;

        float roll = Random.Range(0f, 1f);
        Debug.Log($"[성장 시도] 등급: {GetGradeName(curWeaponData.Rank)}, 확률: {chance * 100f}% → {roll}");

        if (roll <= chance)
        {
            ++curWeaponData.Rank;
            enchantLevel = 0;
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
            enchantLevel = 0;
        }
    }

    void Start()
    {
        Initialize();
        Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
    }

    void Initialize()
    {
        isActivated = false;
        enchantLevel = 0;
    }

    float GetSuccessRate(int level)
    {
        return (level < enchantSuccessRates.Count) ? enchantSuccessRates[level] : enchantSuccessRates[^1];
    }

    float GetGradeMultiplier(int rank)
    {
        return gradeMultipliers.TryGetValue(rank, out float multiplier) ? multiplier : 1.0f;
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
}