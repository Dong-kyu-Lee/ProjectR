using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIManager : MonoBehaviour
{
    [SerializeField]
    private BuffManager buffManager;
    [SerializeField]
    private GameObject buffIconPrefab;
    [SerializeField]
    private Transform buffPanel;
    private Dictionary<BuffType, GameObject> activeBuffIcons = new Dictionary<BuffType, GameObject>();


    private void Start()
    {
        if (buffManager == null)
        {
            GameObject player = GameManager.Instance.CurrentPlayer;
            if (player != null)
                buffManager = player.GetComponent<BuffManager>();
        }

        if (buffPanel == null)
            Debug.LogError("BuffPanel이 할당되지 않았습니다.");
    }

    private void Update()
    {// 테스트용 코드
        if (Input.GetKeyDown(KeyCode.G))
        {
            // BuffType 열거형의 모든 값을 가져와서 랜덤하게 선택
            BuffType[] buffTypes = (BuffType[])System.Enum.GetValues(typeof(BuffType));
            int randomIndex = Random.Range(0, buffTypes.Length);
            BuffType randomBuff = buffTypes[randomIndex];
            Debug.Log("적용할 랜덤 버프/디버프: " + randomBuff);

            if (buffManager != null)
            {
                // 기본 지속시간 10초로 ActivateBuff 호출
                buffManager.ActivateBuff(randomBuff, 10f);
            }
            else
            {
                Debug.LogError("BuffManager가 null 입니다.");
            }
        }
        UpdateBuffUI();
    }

    private void UpdateBuffUI()
    {
        // 1. BuffManager의 활성 버프 정보에 따라 아이콘 생성 또는 업데이트
        foreach (var buffEntry in buffManager.ActiveBuffDict)
        {
            BuffType buffType = buffEntry.Key;
            Buff buffData = buffEntry.Value;

            if (!activeBuffIcons.ContainsKey(buffType))
            {
                // 새 버프 아이콘을 생성하여 buffPanel 아래에 배치
                GameObject newIcon = Instantiate(buffIconPrefab, buffPanel);
                BuffIconUI iconUI = newIcon.GetComponent<BuffIconUI>();
                if (iconUI != null)
                {
                    iconUI.Initialize(buffData);
                }
                activeBuffIcons.Add(buffType, newIcon);
            }
            else
            {
                // 이미 존재하는 아이콘이면 데이터만 갱신
                GameObject iconObj = activeBuffIcons[buffType];
                BuffIconUI iconUI = iconObj.GetComponent<BuffIconUI>();
                if (iconUI != null)
                {
                    iconUI.UpdateData(buffData);
                }
            }
        }

        // 2. 만약 PlayerBuffManager에서 해제된 버프가 있다면, 해당 아이콘을 제거
        List<BuffType> iconsToRemove = new List<BuffType>();
        foreach (var iconPair in activeBuffIcons)
        {
            if (!buffManager.ActiveBuffDict.ContainsKey(iconPair.Key))
            {
                Destroy(iconPair.Value);
                iconsToRemove.Add(iconPair.Key);
            }
        }
        foreach (BuffType key in iconsToRemove)
        {
            activeBuffIcons.Remove(key);
        }
    }
}