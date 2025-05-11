using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Status : MonoBehaviour
{
    private float maxHp;
    private float hp;
    private float damage;
    private float damageReduction;
    private float attackSpeed;
    private float additionalAttackSpeed;
    private float totalAttackSpeed;
    private float moveSpeed;
    private float additionalMoveSpeed;
    private float totalMoveSpeed;

    public float MaxHp
    {
        get { return maxHp; }
        set
        {
            maxHp = value;
            if (Hp > maxHp) Hp = maxHp;
        }
    }

    public virtual float Hp
    { 
        get { return hp; }
        set
        {
            hp = value;
            if (hp <= 0f)
            {
                hp = 0f;
                Dead();
            }
            CalcReceiveDamage.Instance.InduranceEffect13_IncreaseDamageReduction();
        }
    }

    public virtual float Damage { get { return damage; } set { damage = value; } }
    public virtual float DamageReduction { get { return damageReduction; } set { damageReduction = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float AdditionalAttackSpeed
    {
        get { return additionalAttackSpeed; }
        set
        {
            additionalAttackSpeed = value;
            totalAttackSpeed = AttackSpeed + (AttackSpeed * additionalAttackSpeed);
        }
    }
    public float TotalAttackSpeed { get { return totalAttackSpeed; } }
    
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public float AdditionalMoveSpeed
    {
        get { return additionalMoveSpeed; }
        set
        {
            additionalMoveSpeed = value;
            if (additionalMoveSpeed < -1f)
            {
                totalMoveSpeed = MoveSpeed + (MoveSpeed * -1);
            }
            else
            {
                totalMoveSpeed = MoveSpeed + (MoveSpeed * additionalMoveSpeed);
            }
        }
    }
    public float TotalMoveSpeed { get { return totalMoveSpeed; } }

    void Start()
    {
        
    }

    void Update()
    {

    }

    // 피해를 입힘. 
    public void TakeDamage(GameObject attacker, float damage, float ignoreDamageReduction, bool isCritical)
    {
        float receiveDamage = (1 - damageReduction * (1 - ignoreDamageReduction)) * damage;

        if (gameObject.CompareTag("Player"))
        {
            // 인내 4레벨.
            if (CalcReceiveDamage.Instance.induranceEffect4)
            {
                receiveDamage -= 1 + 10 * damageReduction;
                if (receiveDamage < 0) receiveDamage = 0;
            }
            // 인내 7레벨.
            if (CalcReceiveDamage.Instance.induranceEffect7 && !CalcDamage.Instance.IsOnCooldown("InduranceEffect7"))
            {
                receiveDamage = 0;
                StartCoroutine(CalcDamage.Instance.Cooldown("InduranceEffect7", 20f));
            }
            // 인내 16레벨.
            if (CalcReceiveDamage.Instance.induranceEffect16 && !CalcDamage.Instance.IsOnCooldown("InduranceEffect16"))
            {
                float damageReductionValue = damage - receiveDamage;
                attacker.GetComponent<EnemyStatus>().TakeTrueDamage(damageReductionValue * 10);
                CalcDamage.Instance.StackDexterityEffect16(attacker);
                StartCoroutine(CalcDamage.Instance.Cooldown("InduranceEffect16", 2f));
            }
        }

        CalcReceiveDamage.Instance.TakeDamageQueue(receiveDamage, isCritical, gameObject);
        Hp -= receiveDamage;
    }

    // 고정 피해를 입힘.
    public void TakeTrueDamage(float damage)
    {
        CalcReceiveDamage.Instance.TakeTrueDamageQueue(damage, gameObject);

        Hp -= damage;
    }

    protected virtual void Dead()
    {

    }
}
