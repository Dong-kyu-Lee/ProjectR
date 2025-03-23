using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    private static InGameUIManager instance;
    public static InGameUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject InGameUIManagerObject = new GameObject("InGameUIManager");
                instance = InGameUIManagerObject.AddComponent<InGameUIManager>();
                DontDestroyOnLoad(InGameUIManagerObject);
            }
            return instance;
        }
    }

    [SerializeField]
    GameObject stopUI;
    [SerializeField]
    GameObject checkUI;
    [SerializeField]
    Text goldText;
    PlayerStatus playerStatus;
    private bool isOpen;

    private void Awake()
    {
        checkUI.SetActive(false);
        stopUI.SetActive(false);
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerStatus = playerObject.GetComponent<PlayerStatus>();
            Debug.Log("할당됨");
        }
        isOpen = false;
        goldText.text = playerStatus.Gold.ToString();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&& isOpen == false)
        {
            stopUI.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.Escape)&&isOpen == true)
        {
            stopUI.SetActive(false);
        }
    }
    public void ContinueButton()
    {
        stopUI.SetActive(false);
    }
    public void FirstToLobby()
    {
        checkUI.SetActive(true);
    }
    public void ToLobby()
    {
        stopUI.SetActive(false);
        checkUI.SetActive(false);
        SceneManager.LoadScene("Lobby + UpgradeScene");
    }
    public void ExitButton()
    {
        Application.Quit();
    }
    public void CancelButton()
    {
        checkUI.SetActive(false);
    }
}
