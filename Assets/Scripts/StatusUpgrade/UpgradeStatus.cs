using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStatus : MonoBehaviour
{
    private int skillPoint;
    private int force;
    private int indurance;
    private int critical;
    private int dexterity;
    private int mystery;

    public int SkillPoint { get { return skillPoint; } set { skillPoint = value; } }
    public int Force { get { return force; } set { force = value; } }
    public int Indurance { get { return indurance; } set { indurance = value; } }
    public int Critical { get { return critical; } set { critical = value; } }
    public int Dexterity { get { return dexterity; } set { dexterity = value; } }
    public int Mystery { get { return mystery; } set { mystery = value; } }

    private HashSet<string> unlockedEffects = new HashSet<string>();  // 중복 실행 방지용

    [SerializeField] private Status status;
    [SerializeField] private StatusEffect statusEffect;

    void Awake()
    {
        skillPoint = 0;
        force = 0;
        indurance = 0;
        critical = 0;
        dexterity = 0;
        mystery = 0;
    }

    private void FixedUpdate()
    {
        Debug.Log("Force : " + force);
    }

    // 스킬 포인트 사용, 업그레이드 스테이터스 증가
    public void IncreaseStat(string statName)
    {
        if (skillPoint <= 0)
        {
            Debug.Log("스킬포인트가 없습니다.");
            return;
        }

        switch (statName)
        {
            case "force":
                force++;
                status.Damage += 1;
                CheckUnlock("force", force);
                break;
            case "indurance":
                indurance++;
                CheckUnlock("indurance", indurance);
                break;
            case "critical":
                critical++;
                CheckUnlock("critical", critical);
                break;
            case "dexterity":
                dexterity++;
                CheckUnlock("dexterity", dexterity);
                break;
            case "mystery":
                mystery++;
                CheckUnlock("mystery", mystery);
                break;
            default:
                Debug.Log("잘못된 스테이터스 이름");
                return;
        }

        skillPoint--;
        Debug.Log($"{statName} 1 증가. 남은 스킬포인트 : {skillPoint}");
    }

    // 스테이터스 초기화
    public void ResetStat()
    {
        status.Damage -= force;
        force = indurance = critical = dexterity = mystery = 0;
        CheckUnlock("force", force);
        CheckUnlock("indurance", indurance);
        CheckUnlock("critical", critical);
        CheckUnlock("dexterity", dexterity);
        CheckUnlock("mystery", mystery);
    }

    // 특수 효과 해금 여부 확인
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