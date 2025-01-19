using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcDamage : MonoBehaviour
{
    public bool critical;

    public bool forceEffect4;
    public bool forceEffect13;
    public bool forceEffect4_Cooldown;

    public bool criticalEffect7;
    public bool criticalEffect7_Cooldown;
    public bool criticalEffect10;
    public bool criticalEffect13;
    public bool criticalEffect16;

    public bool dexterityEffect4;
    public bool dexterityEffect10;
    public bool dexterityEffect10_Cooldown;
    public bool dexterityEffect16;
    public int dexterityEffect16_Stack;

    private static CalcDamage instance;

    public static CalcDamage Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject calcDamageObject = new GameObject("CalcDamage");
                instance = calcDamageObject.AddComponent<CalcDamage>();
                DontDestroyOnLoad(calcDamageObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else if (instance != this)
        {
            Destroy(gameObject); // 중복된 CalcDamage 제거
        }
    }

    // 추가 공격.
    public void CheckAddtionalDamage(PlayerStatus playerStatus, GameObject enemy)
    {
        float damage = playerStatus.TotalDamage;
        float ignoreDamageReduction = playerStatus.IgnoreDamageReduction;

        // 무력 4레벨 & 13레벨.
        if (forceEffect4 && !forceEffect4_Cooldown)
        {
            if (forceEffect13)
            {
                damage = 5 * damage;
                damage = CheckCritical(playerStatus, damage, ref ignoreDamageReduction);
                enemy.GetComponent<Status>().TakeDamage(damage, ignoreDamageReduction);
                DexterityEffect4_TrueDamage(playerStatus, enemy);
                StackDexterityEffect16(playerStatus, enemy);
                StartCoroutine(ForceEffect4_Cooldown(4f));
            }
            else
            {
                damage = 2 * damage;
                damage = CheckCritical(playerStatus, damage, ref ignoreDamageReduction);
                enemy.GetComponent<Status>().TakeDamage(damage, ignoreDamageReduction);
                DexterityEffect4_TrueDamage(playerStatus, enemy);
                StackDexterityEffect16(playerStatus, enemy);
                StartCoroutine(ForceEffect4_Cooldown(5f));
            }
        }

        // 재주 10레벨.
        if (dexterityEffect10 && !dexterityEffect10_Cooldown)
        {
            damage = 0.2f * damage;
            damage = CheckCritical(playerStatus, damage, ref ignoreDamageReduction);
            enemy.GetComponent<Status>().TakeDamage(damage, ignoreDamageReduction);
            DexterityEffect4_TrueDamage(playerStatus, enemy);
            StackDexterityEffect16(playerStatus, enemy);
            StartCoroutine(ForceEffect4_Cooldown(0.5f));
        }
    }

    // 재주 4레벨.
    public void DexterityEffect4_TrueDamage(PlayerStatus playerStatus, GameObject enemy)
    {
        if (dexterityEffect4)
        {
            enemy.GetComponent<Status>().TakeTrueDamage(3);
            StackDexterityEffect16(playerStatus, enemy);
        }
    }

    // 재주 16레벨 스택 계산.
    public void StackDexterityEffect16(PlayerStatus playerStatus, GameObject enemy)
    {
        float damage = playerStatus.TotalDamage;
        float criticalPercnet = playerStatus.CriticalPercent;
        float criticalDamage = playerStatus.CriticalDamage;
        float ignoreDamageReduction = playerStatus.IgnoreDamageReduction;

        if (dexterityEffect16)
        {
            dexterityEffect16_Stack++;
            Debug.Log(dexterityEffect16_Stack);
            if (dexterityEffect16_Stack >= 15)
            {
                dexterityEffect16_Stack -= 15;
                damage = CheckCritical(playerStatus, damage, ref ignoreDamageReduction);
                enemy.GetComponent<Status>().TakeTrueDamage(damage);
                DexterityEffect4_TrueDamage(playerStatus, enemy);
            }
        }
    }

    // 무력 4레벨 쿨타임.
    private IEnumerator ForceEffect4_Cooldown(float cooldown)
    {
        forceEffect4_Cooldown = true;
        yield return new WaitForSeconds(cooldown);
        forceEffect4_Cooldown = false;
    }

    // 치명 7레벨 쿨타임.
    private IEnumerator CriticalEffect7_Cooldown(float cooldown)
    {
        criticalEffect7_Cooldown = true;
        yield return new WaitForSeconds(cooldown);
        criticalEffect7_Cooldown = false;
    }

    // 재주 10레벨 쿨타임.
    private IEnumerator DexterityEffect10_Cooldown(float cooldown)
    {
        dexterityEffect10_Cooldown = true;
        yield return new WaitForSeconds(cooldown);
        dexterityEffect10_Cooldown = false;
    }

    // 공격의 크리티컬 여부 확인.
    public float CheckCritical(PlayerStatus playerStatus, float damage, ref float ignoreDamageReduction)
    {
        float criticalPercent = playerStatus.CriticalPercent;
        float criticalDamage = playerStatus.CriticalDamage;

        if (criticalEffect7 && !criticalEffect7_Cooldown)
        {
            // 크리티컬 증가 버프 추가해야 함.
            Debug.Log("크리티컬 증가");
            StartCoroutine(CriticalEffect7_Cooldown(20f));
        }

        if (criticalEffect16)
        {
            if (criticalPercent >= 100)
            {
                float temp = criticalPercent - 100f;
                criticalDamage += temp;
            }
        }
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < criticalPercent)
        {
            Debug.Log("크리티컬!");
            if (criticalEffect13)
                ignoreDamageReduction = 1 - (1 - playerStatus.IgnoreDamageReduction) * (1 - 0.5f);
            if (criticalEffect10)
            {
                return 1.2f * damage * (1 + criticalDamage * 0.01f);
            }
            else
            {
                return damage * (1 + criticalDamage * 0.01f);
            }
        }
        else
        {
            return damage;
        }
    }
}
