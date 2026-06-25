using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static PlayerObj;

public class PlayerStatus : Status
{
    private float level;
    private float exp;
    private float additionalDamage;
    private float additionalDamageReduction;
    private float criticalPercent;
    private float criticalDamage;
    private float priceAdditional;
    private float totalDamage;
    private float totalDamageReduction;
    private float ignoreDamageReduction;
    private float debuffDamage;
    private float buffDuration;
    private float debuffDuration;

    private List<SlowDebuff> slowDebuffs = new();
    private float baseMoveSpeed;

    // 안전 구역 안내 메시지를 한 번만 띄우기 위한 스위치
    private bool hasShownSafeZoneMessage = false;

    public float Level { get { return level; } set { level = value; } }
    public float CriticalPercent { get { return criticalPercent; } set { criticalPercent = value; } }
    public float CriticalDamage { get { return criticalDamage; } set { criticalDamage = (value < 0.0f) ? 0 : value; } }
    public float PriceAdditional { get { return priceAdditional; } set { priceAdditional = value; } }
    public float TotalDamage { get { return totalDamage; } }
    public float TotalDamageReduction { get { return totalDamageReduction; } }
    public float IgnoreDamageReduction { get { return ignoreDamageReduction; } set { ignoreDamageReduction = value; } }

    public float BuffDuration
    {
        get { return buffDuration; }
        set
        {
            buffDuration = value;
            CalcDamage.Instance.additionalBuffTime = value;
        }
    }

    public float DebuffDuration
    {
        get { return debuffDuration; }
        set
        {
            debuffDuration = value;
            CalcDamage.Instance.additionalDebuffTime = value;
        }
    }

    public float DebuffDamage { get { return debuffDamage; } set { debuffDamage = value; } }
    private float gold;
    public float Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            InGameUIManager.Instance?.CheckGold();
        }
    }

    public float Exp
    {
        get { return exp; }
        set
        {
            exp = value;
        }
    }

    public override float Damage
    {
        get { return base.Damage; }
        set
        {
            base.Damage = value;
            totalDamage = base.Damage + (base.Damage * additionalDamage);
            CalcDamage.Instance.CurseEffect10_IncreaseDebuffDamage(); // 저주 10레벨.
        }
    }

    public override float DamageReduction
    {
        get { return base.DamageReduction; }
        set
        {
            base.DamageReduction = value;
            CalcReceiveDamage.Instance.InduranceEffect10_IncreaseDamage(); // 인내 10레벨.
        }
    }

    // 체력이 깎이기 전에 씬을 검사하여 무시합니다.
    public override float Hp
    {
        get { return base.Hp; }
        set
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // 1. 로비나 연습장 씬일 경우
            if (currentScene == "LobbyScene" || currentScene == "UpgradeTestScene")
            {
                // 체력이 현재보다 깎이는 상황(데미지를 입음)인지 확인
                if (value < base.Hp)
                {
                    // 안내 메시지를 아직 안 띄웠다면 최초 1회 출력
                    if (!hasShownSafeZoneMessage && InGameUIManager.Instance != null)
                    {
                        InGameUIManager.Instance.ShowStatus("안전 구역에서는 체력이 감소하지 않습니다.");
                        hasShownSafeZoneMessage = true;
                    }

                    // 체력 감소를 무시하고 강제 종료 (UI에는 이미 데미지가 뜬 상태)
                    return;
                }
            }

            // 2. 그 외의 경우 (실전 던전이거나, 체력이 회복되는 경우) 정상 적용
            base.Hp = value;
            if (base.Hp <= 0f)
            {
                base.Hp = 0f;
                Dead();
            }
            CalcReceiveDamage.Instance.InduranceEffect13_IncreaseDamageReduction();
        }
    }

    public float AdditionalDamage
    {
        get { return additionalDamage; }
        set
        {
            additionalDamage = value;
            totalDamage = Damage + (Damage * additionalDamage);
            CalcDamage.Instance.CurseEffect10_IncreaseDebuffDamage(); // 저주 10레벨.
        }
    }

    private List<float> damageReductions = new List<float>();

    public float AdditionalDamageReduction
    {
        get { return additionalDamageReduction; }
        set
        {
            additionalDamageReduction = value;
            if (additionalDamageReduction > 0f) AddDamageReduction(additionalDamageReduction);
            else if (additionalDamageReduction < 0f) RemoveDamageReduction(additionalDamageReduction);
            else { }
            additionalDamageReduction = 0;
        }
    }
    // 새로운 피해 감소 요소 추가
    private void AddDamageReduction(float reduction)
    {
        damageReductions.Add(reduction);
        DamageReduction = 1 - damageReductions.Aggregate(1f, (accumulator, reduction) => accumulator * (1 - reduction));
    }

    // 특정 피해 감소 요소 제거
    private void RemoveDamageReduction(float reduction)
    {
        if (!damageReductions.Remove(-reduction))
        {
            damageReductions.Add(reduction);
        }
        DamageReduction = 1 - damageReductions.Aggregate(1f, (accumulator, reduction) => accumulator * (1 - reduction));
    }

    void Awake()
    {
        MaxHp = 100f;
        Hp = MaxHp;
        Damage = 10f;
        DamageReduction = 0;
        AttackSpeed = 1.0f;
        MoveSpeed = 3f;
        Gold = 100f;

        level = 1f;
        exp = 0;
        AdditionalDamageReduction = 0;
        AdditionalAttackSpeed = 0;
        AdditionalMoveSpeed = 0;
        criticalDamage = 0.5f;
        AdditionalDamage = 0;
        criticalPercent = 0;
        priceAdditional = 0;
        ignoreDamageReduction = 0;

        baseMoveSpeed = MoveSpeed;
    }

    private void Start()
    {
        string playerName = GameManager.Instance.CurrentPlayer.GetComponent<PlayerControllerBase>().playerName;
        if (playerName == "blacksmith") DamageReduction = 0.3f;
    }

    private void Update()
    {
        if (slowDebuffs.Count == 0) return;

        // 뒤에서 앞으로 탐색하며 만료된 디버프 제거
        for (int i = slowDebuffs.Count - 1; i >= 0; i--)
        {
            if (Time.time >= slowDebuffs[i].endTime)
                slowDebuffs.RemoveAt(i);
        }

        RecalculateSlow();
    }

    protected override void Dead()
    {
        //gameObject.GetComponent<PlayerController>().Dead();

        // 리팩토링으로 인해 작성된 분기문. V2 완전 이관 후 편집 필요
        BartenderController bartenderController = gameObject.GetComponent<BartenderController>();

        if (bartenderController == null)
        {
            gameObject.GetComponent<PlayerControllerBase>().Dead();
        }
        else
        {
            bartenderController.Dead();
        }


    }

    public void ApplySlow(float amount, float duration)
    {
        // 새 디버프 추가
        slowDebuffs.Add(new SlowDebuff(amount, duration));
        // 즉시 재계산
        RecalculateSlow();
    }

    private void RecalculateSlow()
    {
        float totalSlow = 0f;
        foreach (var debuff in slowDebuffs)
            totalSlow += debuff.amount;

        totalSlow = Mathf.Clamp01(totalSlow); // 최대 100%(=이동 불가) 제한
        MoveSpeed = baseMoveSpeed * (1f - totalSlow);
    }

}