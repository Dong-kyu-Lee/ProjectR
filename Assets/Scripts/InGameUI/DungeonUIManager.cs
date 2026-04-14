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

    void Start()
    {
        GameManager.Sound.Play("Sounds/BGM/NormalDungeonBGM", Sound.Bgm);
    }

    void Update()
    {
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
            fullMap.SetActive(true);
            minimap.SetActive(false);
            bigMinimap.SetActive(true);
            isFullMapOpen = true;
            isBigMinimapActive = true;
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
    }
}
