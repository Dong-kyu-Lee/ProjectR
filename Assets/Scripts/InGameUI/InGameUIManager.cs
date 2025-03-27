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
    GameObject[] buffUI;
    [SerializeField]
    GameObject[] debuffUI;
    [SerializeField]
    Text goldText;
    PlayerStatus playerStatus;
    PlayerBuffManager playerBuffManager;
    [SerializeField]
    Slider HpBarSlider;
    [SerializeField]
    Text hpTxt;
    private bool isOpen;

    private void Awake()
    {
        //buffUI.SetActive(false);
        //debuffUI.SetActive(false);
        checkUI.SetActive(false);
        stopUI.SetActive(false);
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerStatus = playerObject.GetComponent<PlayerStatus>();
            playerBuffManager = playerObject.GetComponent<PlayerBuffManager>();
        }
        isOpen = false;
        hpTxt.text = playerStatus.Hp.ToString()+"/"+playerStatus.MaxHp.ToString();
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


    private void CheckHp() //*HP 갱신
    {
        if (HpBarSlider != null)
            hpTxt.text = playerStatus.Hp.ToString()+"/"+playerStatus.MaxHp.ToString();
            HpBarSlider.value = playerStatus.Hp /playerStatus.MaxHp ;
    }
    private void Damage(float damage)
    {
        if (playerStatus.MaxHp == 0 || playerStatus.Hp <= 0) //* 이미 체력 0이하면 패스
            return;
        playerStatus.Hp -= damage;
        CheckHp(); //* 체력 갱신
        if (playerStatus.Hp <= 0)
        {
            //사망
        }
    }
    private void BuffUI()
    {
        
    }
    private void DebuffUI()
    {

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
