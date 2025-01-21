using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    private float hp;
    private float damage;
    private float damageReduction;
    private float attackSpeed;
    private float moveSpeed;

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
    public float DamageReduction { get { return damageReduction; } set { damageReduction = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage, float ignoreDamageReduction)
    {
        Debug.Log("Attack " + (1 - (damageReduction * (1 - ignoreDamageReduction))) * damage);
        Debug.Log(ignoreDamageReduction);
        Hp -= (1 - (damageReduction * (1 - ignoreDamageReduction))) * damage;
    }

    public void TakeTrueDamage(float damage)
    {
        Debug.Log("True Attack " + damage);
        Hp -= damage;
    }

    protected virtual void Dead()
    {

    }
}
