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
    public float DamageReduction { get { return damageReduction; } set { damageReduction = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private Dictionary<GameObject, Queue<float>> damageQueue = new Dictionary<GameObject, Queue<float>>();
    private Dictionary<GameObject, Queue<int>> damageTypeQueue = new Dictionary<GameObject, Queue<int>>();
    private Dictionary<GameObject, bool> isProcessing = new Dictionary<GameObject, bool>();
    private float damageInterval = 0.05f;

    // 피해를 입힘. 
    public void TakeDamage(float damage, float ignoreDamageReduction, bool isCritical)
    {
        float receiveDamage = (1 - damageReduction * (1 - ignoreDamageReduction)) * damage;

        // 데미지 텍스트 출력 준비.
        if (!damageQueue.ContainsKey(gameObject))
        {
            damageQueue[gameObject] = new Queue<float>();
            damageTypeQueue[gameObject] = new Queue<int>();
            isProcessing[gameObject] = false;
        }

        damageQueue[gameObject].Enqueue(receiveDamage); // 데미지를 큐에 삽입.
        if (isCritical) damageTypeQueue[gameObject].Enqueue(1); // 크리티컬 여부에 따라 데미지 타입을 큐에 삽입.
        else damageTypeQueue[gameObject].Enqueue(0);


        if (!isProcessing[gameObject])
        {
            StartCoroutine(ProcessDamageQueue(gameObject));
        }

        Hp -= receiveDamage; // 체력 감소.
    }

    // 고정 피해를 입힘.
    public void TakeTrueDamage(float damage)
    {
        if (!damageQueue.ContainsKey(gameObject))
        {
            damageQueue[gameObject] = new Queue<float>();
            damageTypeQueue[gameObject] = new Queue<int>();
            isProcessing[gameObject] = false;
        }

        damageQueue[gameObject].Enqueue(damage); // 데미지를 큐에 삽입.
        damageTypeQueue[gameObject].Enqueue(2); // 고정 피해 데미지 타입을 큐에 삽입.

        if (!isProcessing[gameObject])
        {
            StartCoroutine(ProcessDamageQueue(gameObject));
        }

        Hp -= damage;
    }

    // 일정 시간 간격으로 데미지 출력.
    private IEnumerator ProcessDamageQueue(GameObject gameObject)
    {
        isProcessing[gameObject] = true;

        while (damageQueue[gameObject].Count > 0)
        {
            float damage = damageQueue[gameObject].Dequeue();
            float damageType = damageTypeQueue[gameObject].Dequeue();

            GameObject damageTextObj = ObjectPoolManager.Instance.GetDamageText();
            damageTextObj.transform.position = transform.position;
            if (damageType == 0) damageTextObj.GetComponent<DamageText>().damageText.color = Color.white;
            else if (damageType == 1) damageTextObj.GetComponent<DamageText>().damageText.color = Color.red;
            else if (damageType == 2) damageTextObj.GetComponent<DamageText>().damageText.color = Color.yellow;
            damageTextObj.GetComponent<DamageText>().SetText(damage.ToString());

            yield return new WaitForSeconds(damageInterval);
        }

        isProcessing[gameObject] = false;
    }

    protected virtual void Dead()
    {

    }
}
