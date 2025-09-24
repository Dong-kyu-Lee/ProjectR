using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public float backgroundSpeed;
    public float frontBackgroundSpeed;
    public GameObject background;
    public GameObject frontBackground;

    void Update()
    {
        // 배경 오브젝트 움직임
        if(background.transform.position.x <= -24)
        {
            background.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            background.transform.Translate(Vector3.left * backgroundSpeed * Time.deltaTime);
        }
        if(frontBackground.transform.position.x <= -24)
        {
            frontBackground.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            frontBackground.transform.Translate(Vector3.left * frontBackgroundSpeed * Time.deltaTime);
        }
    }

    // 게임 시작 화면에서 Start 버튼에 의해 호출되는 이벤트 함수
    public void StartGame()
    {
        if (string.IsNullOrEmpty(DungeonTestHelper.Instance.testSceneName))
            SceneManager.LoadScene("LobbyScene");
        else
        {
            DungeonTestHelper.Instance.LoadTestScene();
        }
    }

    // 게임 종료 버튼에 의해 호출되는 이벤트 함수
    public void ExitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
