using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


// 본 스크립트는 아이템 수류탄 용 디버프 장판을 임시로 정의하는 스크립트임.
// 해당 스크립트는 로직의 수정이 좀 필요해보임.
public class TempDebuffArea : MonoBehaviour
{
    [SerializeField]
    private Vector2 size = new Vector2(10, 0.5f);       //장판오브젝트의 크기
    [SerializeField]
    private float maxTimeToDelete = 15.0f;              //임시생성된 장판이 사라지기까지의 시간
    [SerializeField]
    private float maxTimeToDebuff = 10.0f;              //대상에게 디버프가 부여될때까지 걸리는 시간
    [SerializeField] private BuffType buffType;         //부여될 디버프 타입
    
    private WaitForSeconds waitForZeroPointOneSec = new WaitForSeconds(0.1f);   //0.1초짜리 WaitForSeconds 객체
    private Dictionary<GameObject, float> debuffCounters = new Dictionary<GameObject, float>(); //해당 장판위에 있는 대상들의 디버프 카운터
    private Coroutine checkCounterCoroutine = null; //디버프 카운터들을 증감시키는 코루틴 변수


    public Vector2 Size
    {
        get { return size; }
        set 
        {
            size.x = value.x < 0.0f ? 0.0f : value.x;
            size.y = value.y < 0.0f ? 0.0f : value.y;
        }
    }

    private void Awake()
    {
        transform.localScale = size;
        StartCoroutine(SetTimerToDelete());
    }

    //장판을 삭제시키는 코루틴
    private IEnumerator SetTimerToDelete()
    {
        yield return new WaitForSeconds(maxTimeToDelete);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            debuffCounters.Add(collision.gameObject, 0.0f);
        }
        if(checkCounterCoroutine == null)
            checkCounterCoroutine = StartCoroutine(UpdateDebuffCounter());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        debuffCounters.Remove(collision.gameObject);
        if(debuffCounters.Count == 0 && checkCounterCoroutine != null) 
        {
            StopCoroutine(checkCounterCoroutine);
            checkCounterCoroutine = null;
        }
    }

    private IEnumerator UpdateDebuffCounter()
    {
        while(debuffCounters.Count > 0)
        {
            for (int i = 0; i < debuffCounters.Count; i++)
            {
                GameObject key = debuffCounters.ElementAt(i).Key;
                if (debuffCounters[key] < maxTimeToDebuff)
                {
                    debuffCounters[key] += 0.1f;
                }
                else
                {
                    key.GetComponent<BuffManager>().ActivateBuff(buffType, maxTimeToDebuff);
                    debuffCounters[key] = 0.0f;
                }
            }
            yield return waitForZeroPointOneSec;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}