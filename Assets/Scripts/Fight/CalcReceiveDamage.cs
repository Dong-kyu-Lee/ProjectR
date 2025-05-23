using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CalcReceiveDamage : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float induranceEffect10_IncreaseDamage;
    public float induranceEffect13_Reduction = 0f;

    public bool induranceEffect4;
    public bool induranceEffect7;
    public bool induranceEffect10;
    public bool induranceEffect13;
    public bool induranceEffect16;

    private static CalcReceiveDamage instance;

    public static CalcReceiveDamage Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject calcReceiveDamageObject = new GameObject("CalcReceiveDamage");
                instance = calcReceiveDamageObject.AddComponent<CalcReceiveDamage>();
                DontDestroyOnLoad(calcReceiveDamageObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else if (instance != this)
        {
            Destroy(gameObject); // 중복된 CalcReceiveDamage 제거
        }

        GameObject player = GameObject.FindWithTag("Player");
        playerStatus = player.GetComponent<PlayerStatus>();
    }

    private Dictionary<GameObject, Queue<float>> damageQueue = new Dictionary<GameObject, Queue<float>>();
    private Dictionary<GameObject, Queue<int>> damageTypeQueue = new Dictionary<GameObject, Queue<int>>();
    private Dictionary<GameObject, bool> isProcessing = new Dictionary<GameObject, bool>();
    private float damageInterval = 0.05f;

    // 피해를 입힐 때 데미지를 큐에 삽입.
    public void TakeDamageQueue(float receiveDamage, bool isCritical, GameObject target)
    {
        // 데미지 텍스트 출력 준비.
        if (!damageQueue.ContainsKey(target))
        {
            damageQueue[target] = new Queue<float>();
            damageTypeQueue[target] = new Queue<int>();
            isProcessing[target] = false;
        }

        damageQueue[target].Enqueue(receiveDamage); // 데미지를 큐에 삽입.
        if (isCritical) damageTypeQueue[target].Enqueue(1); // 크리티컬 여부에 따라 데미지 타입을 큐에 삽입.
        else damageTypeQueue[target].Enqueue(0);


        if (!isProcessing[target])
        {
            StartCoroutine(ProcessDamageQueue(target));
        }
    }

    // 고정 피해를 입힐 때 데미지를 큐에 삽입.
    public void TakeTrueDamageQueue(float damage, GameObject target)
    {
        if (!damageQueue.ContainsKey(target))
        {
            damageQueue[target] = new Queue<float>();
            damageTypeQueue[target] = new Queue<int>();
            isProcessing[target] = false;
        }

        damageQueue[target].Enqueue(damage); // 데미지를 큐에 삽입.
        damageTypeQueue[target].Enqueue(2); // 고정 피해 데미지 타입을 큐에 삽입.

        if (!isProcessing[target])
        {
            StartCoroutine(ProcessDamageQueue(target));
        }
    }

    // 디버프 피해를 입힐 때 데미지를 큐에 삽입.
    public void TakeDebuffDamageQueue(float damage, GameObject target)
    {
        if (!damageQueue.ContainsKey(target))
        {
            damageQueue[target] = new Queue<float>();
            damageTypeQueue[target] = new Queue<int>();
            isProcessing[target] = false;
        }

        damageQueue[target].Enqueue(damage); // 데미지를 큐에 삽입.
        damageTypeQueue[target].Enqueue(3); // 디버프 피해 데미지 타입을 큐에 삽입.

        if (!isProcessing[target])
        {
            StartCoroutine(ProcessDamageQueue(target));
        }
    }

    // 일정 시간 간격으로 큐에서 데미지 출력.
    private IEnumerator ProcessDamageQueue(GameObject gameObject)
    {
        isProcessing[gameObject] = true;

        while (damageQueue[gameObject].Count > 0)
        {
            float damage = damageQueue[gameObject].Dequeue();
            float damageType = damageTypeQueue[gameObject].Dequeue();

            GameObject damageTextObj = ObjectPoolManager.Instance.GetDamageText();
            damageTextObj.transform.position = gameObject.transform.position;
            if (damageType == 0) damageTextObj.GetComponent<DamageText>().damageText.color = Color.white;
            else if (damageType == 1) damageTextObj.GetComponent<DamageText>().damageText.color = Color.red;
            else if (damageType == 2) damageTextObj.GetComponent<DamageText>().damageText.color = Color.yellow;
            else if (damageType == 3) damageTextObj.GetComponent<DamageText>().damageText.color = Color.blue;
            damageTextObj.GetComponent<DamageText>().SetText(damage.ToString());

            yield return new WaitForSeconds(damageInterval);
        }

        isProcessing[gameObject] = false;
    }

    // 인내 10레벨 피해량 증가.
    public void InduranceEffect10_IncreaseDamage()
    {
        if (induranceEffect10)
        {
            playerStatus.AdditionalDamage -= induranceEffect10_IncreaseDamage;
            induranceEffect10_IncreaseDamage = playerStatus.DamageReduction;
            playerStatus.AdditionalDamage += induranceEffect10_IncreaseDamage;
        }
        else
        {
            playerStatus.AdditionalDamage -= induranceEffect10_IncreaseDamage;
            induranceEffect10_IncreaseDamage = 0;
        }
    }

    // 인내 13레벨 피해량 감소 증가.
    public void InduranceEffect13_IncreaseDamageReduction()
    {
        if (induranceEffect13)
        {
            float hpRatio = playerStatus.Hp / playerStatus.MaxHp;
            playerStatus.AdditionalDamageReduction -= induranceEffect13_Reduction;

            if (hpRatio < 0.3f) induranceEffect13_Reduction = 0.3f;
            else if (hpRatio < 0.5f) induranceEffect13_Reduction = 0.2f;
            else if (hpRatio < 0.7f) induranceEffect13_Reduction = 0.1f;
            else induranceEffect13_Reduction = 0f;

            playerStatus.AdditionalDamageReduction += induranceEffect13_Reduction;
        }
        else
        {
            playerStatus.AdditionalDamageReduction -= induranceEffect13_Reduction;
            induranceEffect13_Reduction = 0f;
        }
    }
}
