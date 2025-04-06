using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIManager : MonoBehaviour
{
    // 버프 정보를 관리할 딕셔너리 (버프 타입 → BuffInfo)
    private Dictionary<BuffType, BuffInfo> buffInfoMapping;

    // 버프 아이콘 프리팹 또는 UI 컨테이너 등 (미리 설정)
    [SerializeField]
    private Transform buffPanel;
    [SerializeField]
    private GameObject buffIconPrefab;

    void Awake()
    {
        // Resources 폴더 내의 모든 BuffInfo 에셋을 로드해서 딕셔너리 구축
        BuffInfo[] infos = Resources.LoadAll<BuffInfo>("BuffInfo");
        buffInfoMapping = new Dictionary<BuffType, BuffInfo>();
        foreach (BuffInfo info in infos)
        {
            if (!buffInfoMapping.ContainsKey(info.buffType))
            {
                buffInfoMapping.Add(info.buffType, info);
            }
        }
    }

    // 예시: 활성 버프를 기반으로 UI 아이콘 생성 및 업데이트
    public void UpdateBuffUI(Dictionary<BuffType, Buff> activeBuffs)
    {
        // 활성화된 버프 데이터(activeBuffs)는 기존 Buff 시스템에서 관리하는 딕셔너리
        foreach (var kvp in activeBuffs)
        {
            BuffType type = kvp.Key;
            Buff buffData = kvp.Value;
            if (buffInfoMapping.TryGetValue(type, out BuffInfo info))
            {
                // info.buffIcon, info.buffName, info.description 등 사용
                // 예시: 새로운 아이콘 프리팹 생성 및 설정
                GameObject iconObj = Instantiate(buffIconPrefab, buffPanel);
                BuffIconUI iconUI = iconObj.GetComponent<BuffIconUI>();
                if (iconUI != null)
                {
                    iconUI.Initialize(info, buffData);
                }
            }
            else
            {
                Debug.LogWarning("등록된 BuffInfo가 없습니다. BuffType : " + type.ToString());
            }
        }
    }
}