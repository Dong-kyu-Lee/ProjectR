using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologueManager : MonoBehaviour
{
    // 컷씬 종료 후 로비 씬으로 이동
    private bool hasSeenPrologue = false;

    private void Awake()
    {
        // 프롤로그 데이터 초기화
        if (PlayerPrefs.HasKey("HasSeenPrologue") == false)
        {
            hasSeenPrologue = false;
            PlayerPrefs.SetInt("HasSeenPrologue", 0);
            PlayerPrefs.Save();
        }
        else
        {
            hasSeenPrologue = PlayerPrefs.GetInt("HasSeenPrologue") == 1 ? true : false;
        }
    }

    // 프롤로그 컷씬이 완료되었을 때 호출되는 함수
    public void CompleteCutScene()
    {
        hasSeenPrologue = true;
        GameManager.Instance.MoveScene(SceneType.LobbyScene, "LobbyScene");
    }
}
