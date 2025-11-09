using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum MissionKind
{
    KILL_ENEMIES,
    MOVE_TO_POINT,
}


public class MissionUI : MonoBehaviour
{
    public Image bgImage;
    public Image directionArrow;
    public TextMeshProUGUI description; // 미션 설명 (ex. "적을 처치하세요.")
    public TextMeshProUGUI killCountText; // "(처치한 적 수 / 총 적 수)"
    public Button openButton;
    public Button closeButton;
    private MissionKind currentMissionKind;
    private Vector3 targetPoint;

    [SerializeField] private Animator bgAnimator;
    [SerializeField] private MissionUI_BG bgScript; // 배경 애니메이션 이벤트를 받기 위한 클래스
    private bool isOpen = false;

    void Awake()
    {
        openButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
        killCountText.gameObject.SetActive(false);
        directionArrow.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        // 이동 미션일 때 방향 화살표 업데이트
        if (currentMissionKind == MissionKind.MOVE_TO_POINT)
        {
            Vector3 dir = targetPoint - GameManager.Instance.CurrentPlayer.transform.position;
            dir.Normalize();
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            directionArrow.rectTransform.rotation = Quaternion.Euler(0, 0, -angle);
        }
    }

    #region Add Mission Methods
    // 적 처치 미션 추가
    public void StartMission(string desc, int killedEnemy, int totalEnemy)
    {
        currentMissionKind = MissionKind.KILL_ENEMIES;
        description.text = desc;
        killCountText.text = "(" + killedEnemy.ToString() + " / " + totalEnemy.ToString() + ")";
        directionArrow.gameObject.SetActive(false);
        StartCoroutine(OpenAndCloseCoroutine());
    }

    // 이동 미션 추가
    public void StartMission(string desc, Vector3 targetPoint)
    {
        currentMissionKind = MissionKind.MOVE_TO_POINT;
        description.text = desc;
        killCountText.gameObject.SetActive(false);
        this.targetPoint = targetPoint;
        directionArrow.gameObject.SetActive(true);
        StartCoroutine(OpenAndCloseCoroutine());
    }
    #endregion

    public void SetKillCountText(int killedEnemy, int totalEnemy)
    {
        killCountText.text = "(" + killedEnemy.ToString() + " / " + totalEnemy.ToString() + ")";
    }

    #region Button interaction Methods
    // 미션 열기(<<) 버튼 클릭 시 호출
    public void OpenMissionUI()
    {
        // background On 애니메이션 실행
        bgAnimator.SetBool("doMissionOn", true);
        openButton.gameObject.SetActive(false);
        // 애니메이션 종료 시 호출될 이벤트 함수 등록
        bgScript.onAnimationEnd += OnAnimationEnd;
        isOpen = true;
    }

    // 미션 닫기(>>) 버튼 클릭 시 호출
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
        if (isOpen)
        {
            // close 버튼 활성화
            closeButton.gameObject.SetActive(true);
            // 미션 설명과 처치한 적 수 텍스트 활성화
            switch (currentMissionKind)
            {
                case MissionKind.KILL_ENEMIES:
                    description.gameObject.SetActive(true);
                    killCountText.gameObject.SetActive(true);
                    break;
                case MissionKind.MOVE_TO_POINT:
                    description.gameObject.SetActive(true);
                    killCountText.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        else
        {
            // open 버튼 활성화
            openButton.gameObject.SetActive(true);
            // 미션 설명과 처치한 적 수 텍스트 비활성화
        }
    }

    IEnumerator OpenAndCloseCoroutine()
    {
        OpenMissionUI();
        yield return new WaitForSeconds(2.3f);
        CloseMissionUI();
    }
    #endregion
}
