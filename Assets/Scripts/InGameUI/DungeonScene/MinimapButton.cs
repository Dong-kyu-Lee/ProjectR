using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// DungeonUIManager의 큰 미니맵 상태에 맞춰 확대 버튼 이미지, 축소 버튼 이미지로 바뀌도록 동작하기 위한 클래스
// 버튼 클릭, Tab 키 등 전환 경로와 무관하게 항상 상태와 일치한다.
public class MinimapButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] DungeonUIManager dungeonUIManager;
    [SerializeField] Sprite zoomInButtonSprite; // 버튼이 ZoomIn 이미지 => 던전 전체 구조 표시
    [SerializeField] Sprite zoomOutButtonSprite;// 버튼이 ZoomOut 이미지 => 현재 방 확대 표시

    void Awake()
    {
        if(button == null) {
            Debug.LogError("MinimapButton 부재");
            return;
        }
        if(image == null)
        {
            image = button.gameObject.GetComponent<Image>();
            if(image == null) Debug.LogWarning("MinimapButton의 UI Image가 없습니다.");
        }
        if(zoomInButtonSprite == null || zoomOutButtonSprite == null)
        {
            Debug.LogWarning("MinimapButton에 할당할 UI Sprite가 없습니다.");
        }
        if(dungeonUIManager == null) dungeonUIManager = FindObjectOfType<DungeonUIManager>(true);
        if(dungeonUIManager == null) Debug.LogWarning("MinimapButton에서 DungeonUIManager를 찾을 수 없습니다.");
    }

    void OnEnable()
    {
        if(dungeonUIManager == null) return;
        dungeonUIManager.OnBigMinimapChanged += UpdateSprite;
        UpdateSprite(dungeonUIManager.IsBigMinimapActive);
    }

    void OnDisable()
    {
        if(dungeonUIManager != null) dungeonUIManager.OnBigMinimapChanged -= UpdateSprite;
    }

    // 확대된 현재 방이 보이면 ZoomOut 이미지, 던전 전체 구조가 보이면 ZoomIn 이미지
    private void UpdateSprite(bool isBigMinimapActive)
    {
        if(image == null) return;
        Sprite target = isBigMinimapActive ? zoomOutButtonSprite : zoomInButtonSprite;
        if(target != null) image.sprite = target;
    }
}
