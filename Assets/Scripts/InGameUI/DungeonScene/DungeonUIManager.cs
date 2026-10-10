using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DungeonUIManager : MonoBehaviour
{
    [SerializeField] private GameObject fullMap;
    public GameObject FullMap{get { return fullMap;} }
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject bigMinimap; // FullMap에서 확대된 현재 방 UI
    public MissionUI missionUI; // Stage에서 미션 관련 기능 접근 시 사용

    private bool isFullMapOpen;
    private bool isBigMinimapActive = false; // 큰 미니맵 활성화 여부
    public bool IsBigMinimapActive => isBigMinimapActive;

    // 확대된 현재 방(true) / 던전 전체 구조(false) 전환 시 발행. 버튼 아이콘 동기화에 사용
    public event System.Action<bool> OnBigMinimapChanged;

    void Start()
    {
        SoundManager.Instance.Play("Sounds/BGM/NormalDungeonBGM", Sound.Bgm);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isFullMapOpen) ShowFullMap();              // 닫혀 있으면 열기 (확대된 현재 방)
            else SwitchBigMinimapAndFullmap();              // 열려 있으면 확대 방 ↔ 던전 전체 구조 전환
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isFullMapOpen) // 지도가 열려있다면
            {
                CloseFullMap();
            }
        }
    }

    public void ShowFullMap()
    {
        if (fullMap != null)
        {
            // fullMap 활성화 시 자식(MinimapButton)이 OnEnable에서 상태를 읽으므로 먼저 갱신
            isFullMapOpen = true;
            isBigMinimapActive = true;
            fullMap.SetActive(true);
            minimap.SetActive(false);
            bigMinimap.SetActive(true);
            OnBigMinimapChanged?.Invoke(isBigMinimapActive);
        }
    }

    public void CloseFullMap()
    {
        if (minimap != null)
        {
            minimap.SetActive(true);
            fullMap.SetActive(false);

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            isFullMapOpen=false;
        }
    }

    public void SwitchBigMinimapAndFullmap()
    {
        isBigMinimapActive = !isBigMinimapActive;
        bigMinimap.SetActive(isBigMinimapActive);
        OnBigMinimapChanged?.Invoke(isBigMinimapActive);
    }
}
