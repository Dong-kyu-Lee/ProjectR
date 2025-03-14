using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameManagerObject = new GameObject("GameManager");
                instance = gameManagerObject.AddComponent<GameManager>();
                DontDestroyOnLoad(gameManagerObject);
            }
            return instance;
        }
    }

    [SerializeField]
    private GameObject playerObject;
    public GameObject CurrentPlayer { get => playerObject; }

    public bool isCreateEnemies;

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복된 GameManager 제거
        }

        if(playerObject == null)
        {
            Debug.LogError("There is not Player Object");
            // 플레이어 프리팹 내 카메라를 받아오도록 변경.
            //cameraObject.transform.SetParent(currentPlayer.transform);
        }
        DontDestroyOnLoad(playerObject);
    }

    // 플레이어 오브젝트를 지정된 시작 지점(position)에 배치하는 함수
    public void PlacePlayerObject(Vector3 position)
    {
        playerObject.transform.position = position;
    }
}
