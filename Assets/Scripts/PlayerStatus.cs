using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float Level { get { return level; } set { level = value; } }
    public float CriticalPercent { get { return criticalPercent; } set { criticalPercent = value; } }
    public float CriticalDamage { get { return criticalDamage; } set { criticalDamage = (value < 0.0f) ? 0 : value;  } }
    public float PriceAdditional { get { return priceAdditional; } set { priceAdditional = value; } }
    public float TotalDamage { get { return totalDamage; } }
    public float TotalDamageReduction { get { return totalDamageReduction; } }
    public float TotalAttackSpeed { get { return totalAttackSpeed; } }

    public float Exp
    {
        get { return exp; }
        set
        {
            exp = value;
        }
    }

    public float AdditionalDamage
    {
        get { return additionalDamage; }
        set
        {
            additionalDamage = value;
            totalDamage = Damage + (Damage * additionalDamage * 0.01f);
            CriticalDamage = totalDamage * 0.5f;
        } 
    }

    public float AdditionalDamageReduction
    {
        get { return additionalDamageReduction; }
        set
        { 
            additionalDamageReduction = value;
            totalDamageReduction = DamageReduction + (DamageReduction * additionalDamageReduction * 0.01f);
        }
    }

    public float AdditionalAttackSpeed
    {
        get { return additionalAttackSpeed; }
        set
        {
            additionalAttackSpeed = value;
            totalAttackSpeed = AttackSpeed - (AttackSpeed * additionalAttackSpeed * 0.01f * 0.5f);
        }
    }

    void Awake()
    {
        Hp = 100f;
        Damage = 0;
        DamageReduction = 0;
        AttackSpeed = 1.5f;
        MoveSpeed = 3f;

        level = 1f;
        exp = 0;
        criticalDamage = 0;
        AdditionalDamage = 0;
        AdditionalDamageReduction = 0;
        AdditionalAttackSpeed = 0;
        totalAttackSpeed = 1.5f;
        criticalPercent = 0;
        priceAdditional = 0;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
