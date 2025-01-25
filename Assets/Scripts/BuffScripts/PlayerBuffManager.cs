using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    private Dictionary<BuffType, Buff> activeBuffDict = new Dictionary<BuffType, Buff>();
    private BuffFactory buffFactory;    //버프 생성을 위한 팩토리 클래스

    private void Start()
    {
        buffFactory = new BuffFactory(gameObject);

        ActivateBuff(BuffType.Posion, 10.0f);
    }

    //BuffType에 해당하는 버프를 생성하고 활성화 시키는 메서드
    public void ActivateBuff(BuffType type, float duration = 10.0f)
    {
        if (isBuffActive(type))
        {
            activeBuffDict[type].BuffOverlap(duration);
            return;
        }

        Buff buff = buffFactory.GenerateBuff(type, duration);
        if (buff == null) {
            Debug.Log("버프 생성 실패");
            return;
        }

        activeBuffDict.Add(type, buff);
        StartCoroutine(StartBuffEffect(type));
    }

    //버프 효과 처리 코루틴
    public IEnumerator StartBuffEffect(BuffType type)
    {
        activeBuffDict[type].ApplyBuffEffect();
        Debug.Log("버프 활성화 : " + type.ToString());

        while (activeBuffDict[type].CurrentDuration > 0.0f)
        {
            activeBuffDict[type].DoActionOnActivate(0.1f);
            yield return new WaitForSeconds(0.1f);
        }
        DeActivateBuff(type);
    }

    //Type에 해당하는 버프를 해제하는 메서드
    public void DeActivateBuff(BuffType type)
    {
        if(activeBuffDict.ContainsKey(type))
        {
            activeBuffDict[type].RemoveBuffEffect();
            activeBuffDict.Remove(type);
        }
        else
        {
            Debug.Log("제거할 버프가 없습니다.");
        }
        Debug.Log("버프 해제 완료" + type.ToString());
    }

    //활성화 된 모든 버프 해제 메서드
    public void DeActivateAllBuffs()
    {
        if (activeBuffDict.Count == 0) return;

        foreach(Buff target in activeBuffDict.Values)
        {
            target.RemoveBuffEffect();
        }
        StopAllCoroutines();
        activeBuffDict.Clear();
    }

    public bool isBuffActive(BuffType type)
    {
        return activeBuffDict.ContainsKey(type);
    }
}