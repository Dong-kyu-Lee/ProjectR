using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffUIManager : MonoBehaviour
{
    // 플레이어의 버프 정보를 관리하는 스크립트를 가져옵니다.
    [SerializeField]
    private PlayerBuffManager playerBuffManager;

    // 버프 아이콘 프리팹 (각 프리팹은 BuffIconUI 컴포넌트를 가져야 함)
    [SerializeField]
    private GameObject buffIconPrefab;

    // 버프 아이콘들이 배치될 컨테이너 (예: Horizontal Layout Group이 적용된 패널)
    [SerializeField]
    private Transform buffPanel;

    // 버프 UI의 배경 이미지 (이 이미지의 RectTransform을 확장시켜 배경 크기를 조절함)
    [SerializeField]
    private Image buffPanelBackground;

    // 현재 활성화된 버프 아이콘들을 관리하는 딕셔너리 (Key: BuffType, Value: 생성된 아이콘 GameObject)
    private Dictionary<BuffType, GameObject> activeBuffIcons = new Dictionary<BuffType, GameObject>();

    // 아이콘 하나의 예상 너비(실제 프리팹 크기에 맞춰 조정)
    [SerializeField]
    private float iconWidth = 100f;
    // 아이콘 사이의 간격
    [SerializeField]
    private float iconSpacing = 5f;

    private void Start()
    {
        // PlayerBuffManager가 인스펙터에 할당되지 않은 경우, 태그로 Player를 찾아 가져옵니다.
        if (playerBuffManager == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerBuffManager = player.GetComponent<PlayerBuffManager>();
        }

        // 버프 패널과 배경이 초기화되어 있어야 합니다.
        if (buffPanel == null)
            Debug.LogError("BuffPanel이 할당되지 않았습니다.");
        if (buffPanelBackground == null)
            Debug.LogError("BuffPanelBackground가 할당되지 않았습니다.");
    }

    private void Update()
    {// 테스트용 코드: G키를 누르면 랜덤하게 버프(또는 디버프)를 플레이어에게 적용함.
        if (Input.GetKeyDown(KeyCode.G))
        {
            // BuffType 열거형의 모든 값을 가져와서 랜덤하게 선택합니다.
            BuffType[] buffTypes = (BuffType[])System.Enum.GetValues(typeof(BuffType));
            int randomIndex = Random.Range(0, buffTypes.Length);
            BuffType randomBuff = buffTypes[randomIndex];
            Debug.Log("적용할 랜덤 버프/디버프: " + randomBuff);

            if (playerBuffManager != null)
            {
                // 기본 지속시간 10초로 ActivateBuff 호출 (필요에 따라 수정 가능)
                playerBuffManager.ActivateBuff(randomBuff, 10f);
            }
            else
            {
                Debug.LogError("PlayerBuffManager가 null 입니다.");
            }
        }

        UpdateBuffUI();
    }

    // 활성 버프의 갱신 및 UI 아이콘 생성/업데이트/제거 처리
    private void UpdateBuffUI()
    {
        // 1. PlayerBuffManager의 활성 버프 정보에 따라 아이콘 생성 또는 업데이트
        foreach (var buffEntry in playerBuffManager.ActiveBuffDict)
        {
            BuffType buffType = buffEntry.Key;
            Buff buffData = buffEntry.Value;

            if (!activeBuffIcons.ContainsKey(buffType))
            {
                // 새 버프 아이콘을 생성하여 buffPanel 아래에 배치합니다.
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
                // 이미 존재하는 아이콘이면 데이터만 갱신합니다.
                GameObject iconObj = activeBuffIcons[buffType];
                BuffIconUI iconUI = iconObj.GetComponent<BuffIconUI>();
                if (iconUI != null)
                {
                    iconUI.UpdateData(buffData);
                }
            }
        }

        // 2. 만약 PlayerBuffManager에서 해제된 버프가 있다면, 해당 아이콘을 제거합니다.
        List<BuffType> iconsToRemove = new List<BuffType>();
        foreach (var iconPair in activeBuffIcons)
        {
            if (!playerBuffManager.ActiveBuffDict.ContainsKey(iconPair.Key))
            {
                Destroy(iconPair.Value);
                iconsToRemove.Add(iconPair.Key);
            }
        }
        foreach (BuffType key in iconsToRemove)
        {
            activeBuffIcons.Remove(key);
        }

        // 3. 버프 아이콘의 수에 맞춰 배경 이미지의 크기를 조정합니다.
        int buffCount = activeBuffIcons.Count;
        float newBackgroundWidth = 0;
        if (buffCount > 0)
        {
            newBackgroundWidth = buffCount * iconWidth + (buffCount - 1) * iconSpacing;
        }
        // 배경 이미지의 RectTransform 업데이트 (높이는 기존 값 유지)
        RectTransform bgRect = buffPanelBackground.GetComponent<RectTransform>();
        if (bgRect != null)
        {
            bgRect.sizeDelta = new Vector2(newBackgroundWidth, bgRect.sizeDelta.y);
        }
    }
}