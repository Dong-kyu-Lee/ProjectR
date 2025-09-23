using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public float backgroundSpeed;
    public float frontBackgroundSpeed;
    public GameObject background;
    public GameObject frontBackground;

    void Update()
    {
        // 배경 오브젝트 움직임
        if (background.transform.position.x <= -20)
        {
            background.transform.position = new Vector3(0, 3, 0);
        }
        else
        {
            background.transform.Translate(Vector3.left * backgroundSpeed * Time.deltaTime);
        }
        if (frontBackground.transform.position.x <= -20)
        {
            frontBackground.transform.position = new Vector3(0, 3, 0);
        }
        else
        {
            frontBackground.transform.Translate(Vector3.left * frontBackgroundSpeed * Time.deltaTime);
        }
    }

    // 게임 종료 함수: 동아리의밤에서만 쓸 함수
    public void EndGame()
    {
        GameManager.Instance.MoveScene(SceneType.StartScene, "StartScene");
    }
}
