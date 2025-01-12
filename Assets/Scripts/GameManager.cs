using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 이 게임 매니저 클래스는 플레이어 오브젝트 생성 및 관리만을 위해 임시로 구현되었습니다.
    // 추후 게임 매니저의 역할을 변경할 수도 있습니다.

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

    public GameObject playerPrefab;
    private GameObject currentPlayer;
    public GameObject CurrentPlayer { get => currentPlayer; }
    public GameObject cameraObject;

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

        if(currentPlayer == null)
        {
            currentPlayer = Instantiate(playerPrefab, new Vector3(0, 0, 0), transform.rotation);
            DontDestroyOnLoad(currentPlayer);
            // 플레이어 프리팹 내 카메라를 받아오도록 변경.
            cameraObject = currentPlayer.GetComponent<PlayerController>().playerCamera.gameObject;
            //cameraObject.transform.SetParent(currentPlayer.transform);
        }
    }

    // 플레이어 오브젝트를 지정된 시작 지점(position)에 배치하는 함수
    public void PlacePlayerObject(Vector3 position)
    {
        currentPlayer.transform.position = position;
    }
}
