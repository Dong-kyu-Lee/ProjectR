using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIManager : MonoBehaviour
{
    private BuffManager buffManager;
    [SerializeField]
    private GameObject buffIconPrefab;
    [SerializeField]
    private Transform buffPanel;
    [SerializeField] 
    private BuffToolTipUI tooltipUI;
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
    {
        // 참조 재연결 buffManager가 없으면 다시 찾기
        if (buffManager == null)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
            {
                // 플레이어에 있는 데이터를 가진 BuffManager를 가져옴
                buffManager = GameManager.Instance.CurrentPlayer.GetComponent<BuffManager>();
            }
        }

        // 여전히 null이라면
        if (buffManager == null)
        {
            if (activeBuffIcons.Count > 0) ClearAllBuffIcons();
            return;
        }

        // G키 테스트 코드
        if (Input.GetKeyDown(KeyCode.G))
        {
            BuffType[] buffTypes = (BuffType[])System.Enum.GetValues(typeof(BuffType));
            buffManager.ActivateBuff(buffTypes[Random.Range(0, buffTypes.Length)], 10f);
        }

        UpdateBuffUI();
    }

    // 아이콘을 모두 지우고 리스트를 비우는 기능
    private void ClearAllBuffIcons()
    {
        foreach (var icon in activeBuffIcons.Values)
        {
            if (icon != null) Destroy(icon);
        }
        activeBuffIcons.Clear();
    }

    private void UpdateBuffUI()
    {
        //데이터 딕셔너리가 유효한지 확인
        if (buffManager == null || buffManager.ActiveBuffDict == null) return;

        // BuffManager의 활성 버프 정보에 따라 아이콘 생성 또는 업데이트
        foreach (var buffEntry in buffManager.ActiveBuffDict)
        {
            BuffType buffType = buffEntry.Key;
            Buff buffData = buffEntry.Value;

            if (!activeBuffIcons.ContainsKey(buffType))
            {
                Debug.Log($"[BuffUIManager] 새 Buff UI 생성 대상: {buffType}");

                // 새 버프 아이콘을 생성하여 buffPanel 아래에 배치
                GameObject newIcon = Instantiate(buffIconPrefab, buffPanel);
                BuffIconUI iconUI = newIcon.GetComponent<BuffIconUI>();
                if (iconUI != null)
                {
                    iconUI.Initialize(buffData);
                    iconUI.SetTooltipReference(tooltipUI);

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

        // 만약 PlayerBuffManager에서 해제된 버프가 있다면, 해당 아이콘을 제거
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