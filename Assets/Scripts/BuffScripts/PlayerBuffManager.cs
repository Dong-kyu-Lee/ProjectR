using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    private Dictionary<BuffType, Buff> activeBuffDict = new Dictionary<BuffType, Buff>();   //현재 적용되고 있는 버프 딕셔너리
    private BuffFactory buffFactory;            //버프 생성을 위한 팩토리 클래스
    private WaitForSeconds nextEffectTime;      //버프 효과 적용 주기를 위한 WaitForSeconds. 동적 생성으로 인한 가비지 줄이기용.
    private float deltaNextEffectTime = 1.0f;   //버프 효과 적용 주기(즉, 틱 간격)

    public Dictionary<BuffType, Buff> ActiveBuffDict { get => activeBuffDict; }

    private void Start()
    {
        buffFactory = new BuffFactory(gameObject);
        nextEffectTime = new WaitForSeconds(1.0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ActivateBuff(BuffType.Drunken, 3.0f);
        }
    }

    //BuffType에 해당하는 버프를 생성하고 활성화 시키는 메서드
    public void ActivateBuff(BuffType type, float totalDuration = 10.0f)
    {
        if (isBuffActive(type))
        {
            activeBuffDict[type].BuffOverlap(totalDuration);
            return;
        }

        Buff buff = buffFactory.GenerateBuff(type, totalDuration);
        if (buff == null) {
            Debug.Log("버프 생성 실패");
            return;
        }

        activeBuffDict.Add(type, buff);
        StartCoroutine(StartBuffEffect(type));
    }

    //활성중인 버프 레벨을 1단계 올리는 메서드
    public void ActiveBuffLevelUpOnce(BuffType type)
    {
        if (isBuffActive(type))
        {
            activeBuffDict[type].BuffOverlap(0.0f);
        }
    }

    //버프 효과 처리 코루틴
    public IEnumerator StartBuffEffect(BuffType type)
    {
        activeBuffDict[type].ApplyBuffEffect();
        Debug.Log("버프 활성화 : " + type.ToString());

        while (activeBuffDict[type].CurrentDuration > 0.0f)
        {
            activeBuffDict[type].DoActionOnActivate(deltaNextEffectTime);
            yield return nextEffectTime;
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
    public void DeActivateBuffsAll()
    {
        if (activeBuffDict.Count == 0) return;

        foreach(Buff target in activeBuffDict.Values)
        {
            target.RemoveBuffEffect();
        }
        StopAllCoroutines();
        activeBuffDict.Clear();
    }

    //해당 버프가 활성화 되있는지 확인하는 함수
    public bool isBuffActive(BuffType type)
    {
        return activeBuffDict.ContainsKey(type);
    }
}