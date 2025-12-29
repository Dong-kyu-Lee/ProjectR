using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonUIManager : MonoBehaviour
{
    public GameObject fullMap;
    public GameObject minimap;
    public MissionUI missionUI; // Stage에서 미션 관련 기능 접근 시 사용

    void Start()
    {
        GameManager.Sound.Play("Sounds/BGM/NormalDungeonBGM", Sound.Bgm);
    }

    public void ShowFullMap()
    {
        if (fullMap != null)
        {
            fullMap.SetActive(true);
            minimap.SetActive(false);
        }
    }

    public void CloseFullMap()
    {
        if (minimap != null)
        {
            minimap.SetActive(true);
            fullMap.SetActive(false);
        }
    }
}
