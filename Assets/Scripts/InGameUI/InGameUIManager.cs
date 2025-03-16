using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject stopUI;
    [SerializeField]
    GameObject checkUI;
    private bool isOpen;
    private void Awake()
    {
        checkUI.SetActive(false);
        stopUI.SetActive(false);
        isOpen = false;
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
    private void ContinueButton()
    {
        stopUI.SetActive(false);
    }
    private void ToLobby()
    {
        stopUI.SetActive(false);

    }
    private void ExitButton()
    {
        Application.Quit();
    }
}
