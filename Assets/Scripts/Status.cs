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
        float receiveDamage = (1 - damageReduction * (1 - ignoreDamageReduction)) * damage;

        if (gameObject.tag == "Enemy")
        {
            CapsuleCollider2D collider = this.GetComponent<CapsuleCollider2D>();
            GameObject damageTextObj = ObjectPoolManager.Instance.GetDamageText();
            float offset = collider.bounds.size.y + 0.4f * (ObjectPoolManager.Instance.activeDamageTexts - 1);
            damageTextObj.transform.position = transform.position + Vector3.up * offset;
            damageTextObj.GetComponent<DamageText>().SetText(receiveDamage.ToString());
        }
        Hp -= receiveDamage;
    }

    public void TakeTrueDamage(float damage)
    {
        Debug.Log("True Attack " + damage);
        if (gameObject.tag == "Enemy")
        {
            CapsuleCollider2D collider = this.GetComponent<CapsuleCollider2D>();
            GameObject damageTextObj = ObjectPoolManager.Instance.GetDamageText();
            float offset = collider.bounds.size.y + 0.4f * (ObjectPoolManager.Instance.activeDamageTexts - 1);
            damageTextObj.transform.position = transform.position + Vector3.up * offset;
            damageTextObj.GetComponent<DamageText>().SetText(damage.ToString());
        }
        Hp -= damage;
    }

    protected virtual void Dead()
    {

    }
}
