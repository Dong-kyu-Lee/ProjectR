using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DungeonUIManager : MonoBehaviour
{
    public GameObject fullMap;
    public GameObject minimap;
    private bool isFullMapOpen;
    public MissionUI missionUI; // Stage에서 미션 관련 기능 접근 시 사용

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
            isFullMapOpen = true;
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
}
