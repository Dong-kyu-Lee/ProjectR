using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{

    private Dictionary<BuffType, Buff> activeBuffList = new Dictionary<BuffType, Buff>();
    private BuffFactory buffFactory;

    private void Start()
    {
        buffFactory = new BuffFactory(gameObject);
        ActivateBuff(BuffType.attackDamageIncrease, 5);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ActivateBuff(BuffType.attackDamageIncrease, 3);

        }
        else if (Input.GetMouseButtonDown(1))
        {
            float dmg = gameObject.GetComponent<PlayerStatus>().AdditionalDamage;
            Debug.Log($"현재 추가 공격력 : {dmg}");
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            DeActivateAllBuffs();
        }

    }

    //BuffType에 해당하는 버프를 활성화 시키는 메서드
    public void ActivateBuff(BuffType type, float duration = 10.0f)
    {
        if (activeBuffList.ContainsKey(type))
        {
            activeBuffList[type].BuffOverlap(duration);
            return;
        }

        Buff buff = buffFactory.GenerateBuff(type, duration);
        if (buff == null) {
            Debug.Log("버프 생성 실패");
            return;
        }

        activeBuffList.Add(type, buff);
        activeBuffList[type].ApplyBuffEffect();
        StartCoroutine(StartBuffTimer(type));
    }

    //버프 지속시간을 카운트하는 코루틴
    public IEnumerator StartBuffTimer(BuffType type)
    {
        Debug.Log("버프 활성화 시작");

        while (activeBuffList[type].CurrentDuration > 0.0f)
        {
            activeBuffList[type].CurrentDuration -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        DeActivateBuff(type);
    }

    //Type에 해당하는 버프를 해제하는 메서드
    public void DeActivateBuff(BuffType type)
    {
        if(activeBuffList.ContainsKey(type))
        {
            activeBuffList[type].RemoveBuffEffect();
            activeBuffList.Remove(type);
            Debug.Log("버프삭제 완료");
        }
        else
        {
            Debug.Log("제거할 버프가 없습니다.");
        }
    }

    public void DeActivateAllBuffs()
    {
        if (activeBuffList.Count == 0) return;

        foreach(Buff target in activeBuffList.Values)
        {
            target.RemoveBuffEffect();
        }
        StopAllCoroutines();
        activeBuffList.Clear();
    }
}