using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerObj;

public class PlayerStatus : Status
{
    private float level;
    private float exp;
    private float additionalDamage;
    private float additionalDamageReduction;
    private float additionalAttackSpeed;
    private float criticalPercent;
    private float criticalDamage;
    private float priceAdditional;
    private float totalDamage;
    private float totalDamageReduction;
    private float totalAttackSpeed;
    private float ignoreDamageReduction;

    public float Level { get { return level; } set { level = value; } }
    public float CriticalPercent { get { return criticalPercent; } set { criticalPercent = value; } }
    public float CriticalDamage { get { return criticalDamage; } set { criticalDamage = (value < 0.0f) ? 0 : value; } }
    public float PriceAdditional { get { return priceAdditional; } set { priceAdditional = value; } }
    public float TotalDamage { get { return totalDamage; } }
    public float TotalDamageReduction { get { return totalDamageReduction; } }
    public float TotalAttackSpeed { get { return totalAttackSpeed; } 
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

    public float AdditionalDamage
    {
        get { return additionalDamage; }
        set
        {
            additionalDamage = value;
            totalDamage = Damage + (Damage * additionalDamage);
        }
    }

    public float AdditionalDamageReduction
    {
        get { return additionalDamageReduction; }
        set
        {
            additionalDamageReduction = value;
            if (additionalDamageReduction >= 0)
                DamageReduction = 1 - (1 - DamageReduction) * (1 - additionalDamageReduction);
            else
                DamageReduction = 1 - (1 - DamageReduction) / (1 + additionalDamageReduction);
            additionalDamageReduction = 0f;
        }
    }

    public float AdditionalAttackSpeed
    {
        get { return additionalAttackSpeed; }
        set
        {
            additionalAttackSpeed = value;
            totalAttackSpeed = AttackSpeed + (AttackSpeed * additionalAttackSpeed);
        }
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
        criticalDamage = 0.5f;
        AdditionalDamage = 0;
        AdditionalDamageReduction = 0;
        AdditionalAttackSpeed = 0;
        totalAttackSpeed = 0.7f;
        criticalPercent = 0;
        priceAdditional = 0;
        ignoreDamageReduction = 0;
    }

    void Start()
    {

    }

    void Update()
    {
        Debug.Log("Damage : " + Damage);
        Debug.Log("Hp : " + MaxHp + " / " + Hp);
    }

    protected override void Dead()
    {
        gameObject.GetComponent<PlayerController>().Dead();
    }
}
