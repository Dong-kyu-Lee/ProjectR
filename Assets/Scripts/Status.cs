using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    private Dictionary<GameObject, Queue<float>> damageQueue = new Dictionary<GameObject, Queue<float>>();
    private Dictionary<GameObject, Queue<int>> damageTypeQueue = new Dictionary<GameObject, Queue<int>>();
    private Dictionary<GameObject, bool> isProcessing = new Dictionary<GameObject, bool>();
    private float damageInterval = 0.05f;

    public void TakeDamage(float damage, float ignoreDamageReduction, bool isCritical)
    {
        float receiveDamage = (1 - damageReduction * (1 - ignoreDamageReduction)) * damage;

        if (!damageQueue.ContainsKey(gameObject))
        {
            damageQueue[gameObject] = new Queue<float>();
            damageTypeQueue[gameObject] = new Queue<int>();
            isProcessing[gameObject] = false;
        }

        damageQueue[gameObject].Enqueue(receiveDamage);
        if (isCritical) damageTypeQueue[gameObject].Enqueue(1);
        else damageTypeQueue[gameObject].Enqueue(0);


        if (!isProcessing[gameObject])
        {
            StartCoroutine(ProcessDamageQueue(gameObject));
        }

        Hp -= receiveDamage;
    }

    public void TakeTrueDamage(float damage)
    {
        if (!damageQueue.ContainsKey(gameObject))
        {
            damageQueue[gameObject] = new Queue<float>();
            damageTypeQueue[gameObject] = new Queue<int>();
            isProcessing[gameObject] = false;
        }

        damageQueue[gameObject].Enqueue(damage);
        damageTypeQueue[gameObject].Enqueue(2);

        if (!isProcessing[gameObject])
        {
            StartCoroutine(ProcessDamageQueue(gameObject));
        }

        Hp -= damage;
    }

    private IEnumerator ProcessDamageQueue(GameObject gameObject)
    {
        isProcessing[gameObject] = true;

        while (damageQueue[gameObject].Count > 0)
        {
            float damage = damageQueue[gameObject].Dequeue(); // 큐에서 하나 가져옴
            float damageType = damageTypeQueue[gameObject].Dequeue();

            GameObject damageTextObj = ObjectPoolManager.Instance.GetDamageText();
            damageTextObj.transform.position = transform.position;
            if (damageType == 0) damageTextObj.GetComponent<DamageText>().damageText.color = Color.white;
            else if (damageType == 1) damageTextObj.GetComponent<DamageText>().damageText.color = Color.red;
            else if (damageType == 2) damageTextObj.GetComponent<DamageText>().damageText.color = Color.yellow;
            damageTextObj.GetComponent<DamageText>().SetText(damage.ToString());

            yield return new WaitForSeconds(damageInterval); // 0.1초 대기
        }

        isProcessing[gameObject] = false;
    }

    protected virtual void Dead()
    {

    }
}
