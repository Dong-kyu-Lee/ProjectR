using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 미니맵 버튼을 누를 때마다 확대 버튼 이미지, 축소 버튼 이미지로 바뀌도록 동작하기 위한 클래스
public class MinimapButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] Sprite zoomInButtonSprite; // 버튼이 ZoomIn 이미지 => 던전 전체 구조 표시
    [SerializeField] Sprite zoomOutButtonSprite;// 버튼이 ZoomOut 이미지 => 현재 방 확대 표시
    private bool isZoomIn = false; // 기본이 ZoomOut.
    
    void Start()
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
        else if(image.sprite != zoomOutButtonSprite) image.sprite = zoomOutButtonSprite;

        button.onClick.AddListener(SwitchButtonSprit);
    }

    void OnDestroy()
    {
        button.onClick?.RemoveListener(SwitchButtonSprit);
    }

    private void SwitchButtonSprit()
    {
        if(isZoomIn) image.sprite = zoomOutButtonSprite;
        else image.sprite = zoomInButtonSprite;
        isZoomIn = !isZoomIn;
    }
}

