using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempDebuffArea : MonoBehaviour
{
    private Dictionary<GameObject, Coroutine> activeDebuffs = new Dictionary<GameObject, Coroutine>();

    [SerializeField]
    private float maxTimeToDelete = 15.0f;              //임시생성된 장판이 사라지기까지의 시간
    [SerializeField] private float interval = 2.0f; // 2초 간격
    [SerializeField] private BuffType buffType;         //부여될 디버프 타입

    private void Awake()
    {
        StartCoroutine(SetTimerToDelete());
    }

    //장판을 삭제시키는 코루틴
    private IEnumerator SetTimerToDelete()
    {
        yield return new WaitForSeconds(maxTimeToDelete);
        // 장판이 사라질 때 모든 대상의 코루틴 강제 종료
        foreach (var target in activeDebuffs)
        {
            if (target.Value != null) StopCoroutine(target.Value);
        }
        activeDebuffs.Clear();

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 대상 태그 확인 및 중복 등록 방지
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (!activeDebuffs.ContainsKey(other.gameObject))
            {
                Coroutine newRoutine = StartCoroutine(ApplyDebuffRoutine(other.gameObject));
                activeDebuffs.Add(other.gameObject, newRoutine);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 영역을 나가면 해당 대상의 코루틴만 찾아서 중단
        if (activeDebuffs.ContainsKey(other.gameObject))
        {
            StopCoroutine(activeDebuffs[other.gameObject]);
            activeDebuffs.Remove(other.gameObject);
        }
    }

    IEnumerator ApplyDebuffRoutine(GameObject target)
    {
        BuffManager buffManager = target.GetComponent<BuffManager>();

        if (buffManager == null) yield break;

        while (true)
        {
            buffManager.ActivateDeBuff(buffType, 10f);
            yield return new WaitForSeconds(interval);
        }
    }
}