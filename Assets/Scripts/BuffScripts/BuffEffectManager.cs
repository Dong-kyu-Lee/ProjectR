using System.Collections;
using System.Collections.Generic;
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
        public GameObject effectPrefab;
    }

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
    public void PlayBuffEffect(BuffType buffType, Vector3 position, Collider col)
    {
        // 등록되지 않은 버프 무시
        if (!buffEffectDict.TryGetValue(buffType, out var buffPrefab))
            return;

        Vector3 spawnPosition = position; // 기본: 발바닥 기준

        // 버프 타입별 위치 보정
        switch (buffType)
        {
            case BuffType.Burn:
                break;

            case BuffType.Poison:
                spawnPosition.y += 1.2f;
                break;
        }

        Instantiate(buffPrefab, spawnPosition, Quaternion.identity);
    }
}
