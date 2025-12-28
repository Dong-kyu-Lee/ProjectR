using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject uiPanel;

    [Header("Target Boss")]
    [SerializeField] private EnemyStatus targetBossStatus;

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f; // 체력이 줄어드는 애니메이션 속도

    void Start()
    {
        if (targetBossStatus != null)
        {
            uiPanel.SetActive(true);
            hpSlider.value = 1f;
        }
        else
        {
            uiPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (targetBossStatus == null) return;

        // 1. 현재 체력 비율 계산 (0.0 ~ 1.0)
        float currentRatio = targetBossStatus.Hp / targetBossStatus.MaxHp;

        // 2. 슬라이더 값을 부드럽게 변경 (Lerp 사용)
        hpSlider.value = Mathf.Lerp(hpSlider.value, currentRatio, Time.deltaTime * lerpSpeed);

        // 3. 보스가 죽으면(비활성화되면) UI 숨기기
        if (targetBossStatus.gameObject.activeSelf == false || targetBossStatus.Hp <= 0)
        {
            uiPanel.SetActive(false);
        }
    }

    // 외부에서 보스를 지정해줄 때 사용하는 함수
    public void SetBoss(EnemyStatus newBoss)
    {
        targetBossStatus = newBoss;
        uiPanel.SetActive(true);
        hpSlider.value = 1f;
    }
}
