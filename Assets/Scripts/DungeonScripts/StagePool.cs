using System.Collections.Generic;
using UnityEngine;

// 한 스테이지 슬롯(1스테이지, 2스테이지 ...)에서 뽑을 수 있는 StageData 후보 묶음
// 예: 1스테이지 풀 = (StageA, StageB, StageC)
[CreateAssetMenu(fileName = "StagePool", menuName = "Stage Assets/StagePool", order = 2)]
public class StagePool : ScriptableObject
{
    [Tooltip("에디터에서 구분하기 위한 이름. 예) 1스테이지 후보")]
    [SerializeField] private string poolName;

    [Tooltip("이 슬롯에서 랜덤으로 선택될 StageData 후보들")]
    public List<StageData> candidates = new List<StageData>();

    public string PoolName { get => string.IsNullOrEmpty(poolName) ? name : poolName; }
    public bool IsValid { get => candidates != null && candidates.Count > 0; }

    // 후보 중 하나를 랜덤 선택하는 함수
    // exclude : 이미 이번 판에서 선택된 StageData. 가능한 한 제외하고 뽑는다.
    public StageData PickRandom(ICollection<StageData> exclude = null)
    {
        if (IsValid == false)
        {
            Debug.LogError($"[{name}] 스테이지 후보가 비어 있음");
            return null;
        }

        List<StageData> pickable = new List<StageData>();
        foreach (StageData data in candidates)
        {
            if (data == null) continue;
            if (exclude != null && exclude.Contains(data)) continue;
            pickable.Add(data);
        }

        // 중복 제외 규칙을 지키면 뽑을 후보가 없는 경우, 중복을 허용해서라도 스테이지를 채움
        if (pickable.Count == 0)
        {
            foreach (StageData data in candidates)
            {
                if (data != null) pickable.Add(data);
            }
        }

        if (pickable.Count == 0)
        {
            Debug.LogError($"[{name}] 유효한 StageData가 없음");
            return null;
        }

        return pickable[Random.Range(0, pickable.Count)];
    }
}
