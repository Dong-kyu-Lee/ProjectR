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

    // 강화 확률 리스트 (0강 ~ 9강)
    List<float> enchantSuccessRates = new List<float> {
        1.0f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.35f, 0.3f
    };

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
        if (enchantLevel >= enchantSuccessRates.Count)
        {
            Debug.Log("최대 강화 단계");
            return;
        }

        float prob = Random.Range(0f, 1f);
        float successRate = GetSuccessRate(enchantLevel);

        Debug.Log($"[강화 시도] 현재 강화 단계: {enchantLevel}, 성공 확률: {successRate * 100f}%, 뽑은 값: {prob}");

        if (prob <= successRate)
        {
            Debug.Log("강화 성공");
            ++enchantLevel;
        }
        else
        {
            Debug.Log("강화 실패");
        }

        onAbilityUpdated.Invoke();
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
}
