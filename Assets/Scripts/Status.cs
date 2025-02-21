using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    private float maxHp;
    private float hp;
    private float damage;
    private float damageReduction;
    private float attackSpeed;
    private float moveSpeed;

    public float MaxHp
    {
        get { return maxHp; }
        set
        {
            maxHp = value;
            if (Hp > maxHp) Hp = maxHp;
        }
    }

    public float Hp
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
        }
    }

    public virtual float Damage { get { return damage; } set { damage = value; } }
    public virtual float DamageReduction { get { return damageReduction; } set { damageReduction = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // 피해를 입힘. 
    public void TakeDamage(float damage, float ignoreDamageReduction, bool isCritical)
    {
        float receiveDamage = (1 - damageReduction * (1 - ignoreDamageReduction)) * damage;

        if (gameObject.CompareTag("Player"))
        {
            if (CalcReceiveDamage.Instance.induranceEffect4) // 인내 4레벨.
                receiveDamage -= 2 + 20 * damageReduction;

            // 인내 7레벨.
            if (CalcReceiveDamage.Instance.induranceEffect7 && !CalcDamage.Instance.IsOnCooldown("InduranceEffect7"))
            {
                receiveDamage = 0;
                StartCoroutine(CalcDamage.Instance.Cooldown("InduranceEffect7", 20f));
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
