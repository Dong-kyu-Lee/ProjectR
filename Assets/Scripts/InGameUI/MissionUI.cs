using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    public Image bgImage;
    public TextMeshProUGUI description; // 미션 설명 (ex. "적을 처치하세요.")
    public TextMeshProUGUI killCountText; // "(처치한 적 수 / 총 적 수)"
    public Button openButton;
    public Button closeButton;

    [Header("Background Image Size")]
    public float openPosX = -100;
    public float openHeight = 210;
    public float closePosX = 60;
    public float closeHeight = 50;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetMissionDescription(string desc)
    {
        description.text = desc;
    }

    public void SetKillCountText(int killed, int total)
    {
        killCountText.text = "(" + killed.ToString() + " / " + total.ToString() + ")";
    }

    // 미션 열기 화살표 버튼 클릭 시 호출
    public void OpenMissionUI()
    {
        // background 이미지 길이를 부드럽게 늘리고 위치 조정

        // open 버튼 비활성화

        // close 버튼 활성화

        // 미션 설명과 처치한 적 수 텍스트 활성화
    }

    // 미션 닫기 화살표 버튼 클릭 시 호출
    public void CloseMissionUI()
    {
        // background 이미지 길이를 부드럽게 줄이고 위치 조정
        // close 버튼 비활성화
        // open 버튼 활성화
        // 미션 설명과 처치한 적 수 텍스트 비활성화
    }
}
