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

    [SerializeField] private Animator bgAnimator;
    [SerializeField] private MissionUI_BG bgScript; // 배경 애니메이션 이벤트를 받기 위한 클래스
    private bool isOpen = false;

    [Header("Background Image Size")]
    public float openPosX = -100;
    public float openHeight = 210;
    public float closePosX = 60;
    public float closeHeight = 50;


    void Awake()
    {
        openButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
        killCountText.gameObject.SetActive(false);
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
        // background On 애니메이션 실행
        bgAnimator.SetBool("doMissionOn", true);
        openButton.gameObject.SetActive(false);
        // 애니메이션 종료 시 호출될 이벤트 함수 등록
        bgScript.onAnimationEnd += OnAnimationEnd;
        isOpen = true;
    }

    // 미션 닫기 화살표 버튼 클릭 시 호출
    public void CloseMissionUI()
    {
        // background Off 애니메이션 실행
        bgAnimator.SetBool("doMissionOn", false);
        closeButton.gameObject.SetActive(false);
        // 애니메이션 종료 시 호출될 이벤트 함수 등록
        bgScript.onAnimationEnd += OnAnimationEnd;
        isOpen = false;
        // 미션 설명과 처치한 적 수 텍스트 비활성화
        description.gameObject.SetActive(false);
        killCountText.gameObject.SetActive(false);
    }

    // 애니메이션 종료 시 호출되는 이벤트 함수
    public void OnAnimationEnd()
    {
        if(isOpen)
        {
            // close 버튼 활성화
            closeButton.gameObject.SetActive(true);
            // 미션 설명과 처치한 적 수 텍스트 활성화
            description.gameObject.SetActive(true);
            killCountText.gameObject.SetActive(true);
        }
        else
        {
            // open 버튼 활성화
            openButton.gameObject.SetActive(true);
            // 미션 설명과 처치한 적 수 텍스트 비활성화
        }
    }
}
