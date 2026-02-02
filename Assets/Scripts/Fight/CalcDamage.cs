using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcDamage : MonoBehaviour
{
    private Dictionary<string, float> skillCooldowns = new Dictionary<string, float>();

    private PlayerStatus playerStatus;
    private BuffManager playerBuffManager;
    private GameObject player;

    public bool fightState;

    public bool forceEffect4;
    public bool forceEffect7;
    public bool forceEffect13;
    public bool forceEffect16;

    public bool criticalEffect4;
    public bool criticalEffect7;
    public bool criticalEffect10;
    public bool criticalEffect13;
    public bool criticalEffect16;

    public bool dexterityEffect4;
    public bool dexterityEffect7;
    public bool dexterityEffect10;
    public bool dexterityEffect13;
    public bool dexterityEffect16;
    public int dexterityEffect16_Stack;

    public bool mysteryEffect7;
    public bool mysteryEffect13;
    public bool mysteryEffect16;

    public float additionalBuffTime = 0f;

    public bool curseEffect4;
    public bool curseEffect10;
    public bool curseEffect13;
    public bool curseEffect16;

    public float additionalDebuffTime = 0f;
    public float curseEffect10_IncreaseDebuffDamage;

    public bool isCritical;

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

        SetPlayer(GameObject.FindWithTag("Player"));
        additionalDebuffTime = 0;
        additionalBuffTime = 0;
    }

    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
        playerStatus = player.GetComponent<PlayerStatus>();
        playerBuffManager = player.GetComponent<BuffManager>();
    }

    // 적 처치 시 획득하는 버프.
    public void KillEnemyBuff()
    {
        if (forceEffect7) playerBuffManager.ActivateBuff(BuffType.Force7, 20.0f);
    }

    // 적 타격시 발동하는 추가 효과.
    public void AdditionalEffect(GameObject enemy)
    {
        DexterityEffect4_TrueDamage(enemy);
        StackDexterityEffect16(enemy);

        if (dexterityEffect7) playerBuffManager.ActivateBuff(BuffType.Dexterity7, 8.0f); // 재주 7레벨 버프.
        if (dexterityEffect13) playerBuffManager.ActivateBuff(BuffType.Dexterity13, 8.0f); // 재주 13레벨 버프.
    }

    // 비활성화된 효과 관련 버프 리셋.
    public void ResetBuff()
    {
        if (!forceEffect7) playerBuffManager.DeactivateBuff(BuffType.Force7);
        if (!forceEffect16) playerBuffManager.DeactivateBuff(BuffType.Force16);
        if (!criticalEffect4) playerBuffManager.DeactivateBuff(BuffType.Critical4);
        if (!criticalEffect7) playerBuffManager.DeactivateBuff(BuffType.Critical7);
        if (!dexterityEffect7) playerBuffManager.DeactivateBuff(BuffType.Dexterity7);
        if (!dexterityEffect13) playerBuffManager.DeactivateBuff(BuffType.Dexterity13);
    }

    // 전투 중 상태 확인(임시).
    public void CheckFightState()
    {
        if (!fightState)
        {
            StartCoroutine(FightState(10f));
        }
        else
        {
            StopCoroutine(FightState(10f));
            StartCoroutine(FightState(10f));
        }
    }

    // 전투 상태(임시).
    IEnumerator FightState(float time)
    {
        float fightStateTime = time;
        fightState = true;
        if (forceEffect16) playerBuffManager.ActivateBuff(BuffType.Force16, 10.0f);
        while (fightStateTime > 0)
        {
            fightStateTime -= Time.deltaTime;
            yield return null;
        }

        fightState = false;
    }

    // 추가 공격.
    public void CheckAddtionalDamage(GameObject enemy)
    {
        float damage = playerStatus.TotalDamage;
        float ignoreDamageReduction = playerStatus.IgnoreDamageReduction;

        // 무력 4레벨 & 13레벨.
        if (forceEffect4 && !IsOnCooldown("ForceEffect4"))
        {
            if (forceEffect13)
            {
                damage = 5 * damage;
                damage = CheckCritical(damage, ref ignoreDamageReduction, ref isCritical);
                enemy.GetComponent<Status>().TakeDamage(player, damage, ignoreDamageReduction, isCritical);
                AdditionalEffect(enemy);
                StartCooldown("ForceEffect4", 4f);
            }
            else
            {
                damage = 2 * damage;
                damage = CheckCritical(damage, ref ignoreDamageReduction, ref isCritical);
                enemy.GetComponent<Status>().TakeDamage(player, damage, ignoreDamageReduction, isCritical);
                AdditionalEffect(enemy);
                StartCooldown("ForceEffect4", 5f);
            }
        }

        // 재주 10레벨.
        if (dexterityEffect10 && !IsOnCooldown("DexterityEffect10"))
        {
            damage = 0.2f * damage;
            damage = CheckCritical(damage, ref ignoreDamageReduction, ref isCritical);
            enemy.GetComponent<Status>().TakeDamage(player, damage, ignoreDamageReduction, isCritical);
            AdditionalEffect(enemy);
            StartCooldown("DexterityEffect10", 0.5f);
        }
    }

    // 재주 4레벨.
    public void DexterityEffect4_TrueDamage(GameObject enemy)
    {
        if (dexterityEffect4)
        {
            float damage = playerStatus.TotalDamage;
            damage = 5 + 0.1f * damage;
            enemy.GetComponent<Status>().TakeTrueDamage(damage);
            StackDexterityEffect16(enemy);
        }
    }

    // 재주 16레벨 스택 계산.
    public void StackDexterityEffect16(GameObject enemy)
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
                damage = 2 * damage;
                damage = CheckCritical(damage, ref ignoreDamageReduction, ref isCritical);
                enemy.GetComponent<Status>().TakeTrueDamage(damage);
                AdditionalEffect(enemy);
            }
        }
    }

    // 쿨타임 계산.
    public void StartCooldown(string skillName, float cooldown)
    {
        skillCooldowns[skillName] = Time.time + cooldown;
    }

    // 쿨타임 확인.
    public bool IsOnCooldown(string skillName)
    {
        if (!skillCooldowns.ContainsKey(skillName))
            return false;

        return Time.time < skillCooldowns[skillName];
    }

    // 공격의 크리티컬 여부 확인.
    public float CheckCritical(float damage, ref float ignoreDamageReduction, ref bool isCritical)
    {
        float criticalPercent = playerStatus.CriticalPercent;
        float criticalDamage = playerStatus.CriticalDamage;
        bool IncCritical = false;

        // 치명 7레벨.
        if (criticalEffect7 && !IsOnCooldown("CriticalEffect7"))
        {
            IncCritical = true;
            playerBuffManager.ActivateBuff(BuffType.Critical7, 1.0f); // 치명 7레벨 버프.
            StartCooldown("CriticalEffect7", 20f);
        }
        if (IncCritical)
        {
            criticalPercent += 1f;
            IncCritical = false;
        }

        // 치명 16레벨.
        if (criticalEffect16)
        {
            if (criticalPercent >= 1)
            {
                float temp = criticalPercent - 1f;
                criticalDamage += temp;
            }
        }
        float randomValue = Random.Range(0f, 1f);

        if (randomValue < criticalPercent)
        {
            isCritical = true;
            if (criticalEffect13) // 치명 13레벨.
                ignoreDamageReduction = 1 - (1 - playerStatus.IgnoreDamageReduction) * (1 - 0.5f);
            if (criticalEffect4) // 치명 4레벨.
                playerBuffManager.ActivateBuff(BuffType.Critical4, 5.0f);
            if (criticalEffect10) // 치명 10레벨.
            {
                return 1.2f * damage * (1 + criticalDamage);
            }
            else
            {
                return damage * (1 + criticalDamage);
            }
        }
        else
        {
            isCritical = false;
            return damage;
        }
    }

    // 저주 10레벨 디버프 피해량 증가.
    public void CurseEffect10_IncreaseDebuffDamage()
    {
        if (curseEffect10)
        {
            playerStatus.DebuffDamage += playerStatus.TotalDamage * 0.005f - curseEffect10_IncreaseDebuffDamage;
            curseEffect10_IncreaseDebuffDamage = playerStatus.TotalDamage * 0.005f;
        }
        else
        {
            playerStatus.DebuffDamage -= curseEffect10_IncreaseDebuffDamage;
            curseEffect10_IncreaseDebuffDamage = 0;
        }
    }
}
    