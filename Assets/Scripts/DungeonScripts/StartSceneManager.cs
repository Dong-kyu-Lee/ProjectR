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

    public void StartGame()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
        SceneManager.LoadScene("LobbyScene");
    }
}
