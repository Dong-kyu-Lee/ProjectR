using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologueManager : MonoBehaviour
{
    // 1. 프롤로그 컷씬 재생
    // 2. 컷씬 종료 후 로비 씬으로 이동
    public bool hasSeenPrologue = false;

    void Start()
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

    // 프롤로그 재생 씬으로 이동하는 함수
    public void StartPrologue()
    {
        SceneManager.LoadScene("Prologue");
    }

    // 프롤로그 컷씬이 완료되었을 때 호출되는 함수
    public void CompleteCutScene()
    {
        GameManager.Instance.MoveScene(SceneType.LobbyScene, "LobbyScene");
    }

    // 프롤로그가 완전히 종료되었음을 기록하는 함수
    public void EndPrologue()
    {
        hasSeenPrologue = true;
        PlayerPrefs.SetInt("HasSeenPrologue", 1);
        PlayerPrefs.Save();
    }
}
