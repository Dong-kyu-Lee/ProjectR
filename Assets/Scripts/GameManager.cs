using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private GameObject playerObject;
    public GameObject CurrentPlayer { 
        get => playerObject;
        set
        {
            playerObject = value;
            DontDestroyOnLoad(playerObject);
        }
    }

    [SerializeField]
    private GameObject upgradeUI;
    [SerializeField]
    private GameObject inventoryUI;
    [SerializeField]
    private GameObject inGameUI;

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
    }

    // 플레이어 오브젝트를 지정된 시작 지점(position)에 배치하는 함수
    public void PlacePlayerObject(Vector3 position)
    {
        playerObject.transform.position = position;
    }

    public void MoveScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            if (sceneName == "DungeonGenerate")
            {
                // 업그레이드UI & 인벤토리 UI 생성
                upgradeUI = Instantiate(Resources.Load<GameObject>("Prefabs/UpgradeUICanvas 1.0"));
                inventoryUI = Instantiate(Resources.Load<GameObject>("Prefabs/Canvas(QuickSlot)"));
                inGameUI = Instantiate(Resources.Load<GameObject>("Prefabs/InGameUICanvas"));
                DontDestroyOnLoad(upgradeUI);
                DontDestroyOnLoad(inventoryUI);
                DontDestroyOnLoad(inGameUI);
            }
            else if (sceneName == "StartScene")
            {
                // 업그레이드UI & 인벤토리 UI 제거
                Destroy(upgradeUI);
                Destroy(inventoryUI);
                Destroy(inGameUI);
                // 플레이어 오브젝트 제거
                Destroy(playerObject);
            }
        }
        SceneManager.LoadScene(sceneName);
    }
}
