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

    public float Level { get { return level; } set { level = value; } }
    public float CriticalPercent { get { return criticalPercent; } set { criticalPercent = value; } }
    public float CriticalDamage { get { return criticalDamage; } set { criticalDamage = (value < 0.0f) ? 0 : value; } }
    public float PriceAdditional { get { return priceAdditional; } set { priceAdditional = value; } }
    public float TotalDamage { get { return totalDamage; } }
    public float TotalDamageReduction { get { return totalDamageReduction; } }
    public float IgnoreDamageReduction { get { return ignoreDamageReduction; } set { ignoreDamageReduction = value; } }
    public float Gold;

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

    public override float Hp
    {
        get { return base.Hp; }
        set
        {
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
        AttackSpeed = 0.7f;
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
    }

    void Start()
    {

    }
    
    protected override void Dead()
    {
        //gameObject.GetComponent<PlayerController>().Dead();

        gameObject.GetComponent<BartenderController>().Dead();

    }
}
