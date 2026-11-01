using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffEffectManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static BuffEffectManager instance;

    public static BuffEffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject BuffEffectManagerObj = new GameObject("BuffEffectManager");
                instance = BuffEffectManagerObj.AddComponent<BuffEffectManager>();
                DontDestroyOnLoad(BuffEffectManagerObj);
            }
            return instance;
        }
    }

    private Dictionary<BuffType, GameObject> buffEffectDict;

    [System.Serializable]
    public class DebuffEffectEntry
    {
        public BuffType buffType;
        public bool isTick;
        public GameObject effectPrefab;
    }

    public Collider2D targetCollider;

    [SerializeField]
    private List<DebuffEffectEntry> buffEffectList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        buffEffectDict = new Dictionary<BuffType, GameObject>();
        // 디버프 이펙트가 있는 버프만 딕셔너리에 등록
        foreach (var entry in buffEffectList)
        {
            buffEffectDict[entry.buffType] = entry.effectPrefab;
        }
    }

    // 버프 이펙트 실행
    public void PlayBuffEffect(BuffType buffType, GameObject target, bool isTick)
    {
        targetCollider = target.GetComponent<Collider2D>();
        if (targetCollider == null)
        {
            Debug.LogError("Target Collider2D를 찾을 수 없습니다!");
            return;
        }

        Bounds bounds = targetCollider.bounds;
        Vector3 bottomPos = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        Vector3 middlePos = bounds.center;
        Vector3 topPos = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

        // 등록되지 않은 버프 무시
        if (!buffEffectDict.TryGetValue(buffType, out var buffPrefab))
            return;

        Vector3 spawnPosition = middlePos; // 기본: 중앙 기준

        // 틱일 경우
        if (isTick)
        {
            // 버프 타입별 위치 보정
            switch (buffType)
            {
                case BuffType.Burn:
                    spawnPosition = bottomPos;
                    break;
                case BuffType.Poison:
                    spawnPosition = middlePos;
                    break;
            }

            Instantiate(buffPrefab, spawnPosition, Quaternion.identity, target.transform);
        }

        // 버스트일 경우
        else
        {
            // 버프 타입별 위치 보정
            switch (buffType)
            {
                case BuffType.Drunken:
                    spawnPosition = topPos;
                    break;

                case BuffType.Freeze:
                    spawnPosition = middlePos;
                    break;

                case BuffType.Curse:
                    spawnPosition = middlePos;
                    break;
            }

            Instantiate(buffPrefab, spawnPosition, Quaternion.identity, target.transform);
        }

    }
}
