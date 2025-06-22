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
                instance = FindObjectOfType<InGameUIManager>();
                if (instance == null)
                    Debug.LogError("InGameUIManager가 씬에 존재하지 않습니다.");
            }
            return instance;
        }
    }

    [SerializeField]
    private GameObject stopUI;
    [SerializeField]
    private GameObject checkUI;
    [SerializeField]
    private Text goldText;
    [SerializeField]
    private Slider HpBarSlider;
    [SerializeField]
    private Text hpTxt;
    [SerializeField]
    private BuffToolTipUI tooltipUI;
    [SerializeField]
    private Image PlayerHead;
    [SerializeField]
    private CharacterUIManager CharacterUI;
    

    public PlayerStatus playerStatus;

    private void Awake()
    {
        checkUI.SetActive(false);
        stopUI.SetActive(false);
    }
    private void Start()
    {
        StartCoroutine(InitPlayerStatus());
    }

    private IEnumerator InitPlayerStatus()
    {
        yield return new WaitUntil(() => GameManager.Instance.CurrentPlayer != null);

        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        if (playerStatus == null)
        {
            Debug.LogWarning("PlayerStatus 컴포넌트를 찾을 수 없습니다.");
            yield break;
        }

        CharacterUI?.InitUIForCurrentPlayer();

        CheckGold();
        UpdateHpSmooth(playerStatus.Hp, playerStatus.MaxHp);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (tooltipUI != null && tooltipUI.IsVisible())
            {
                tooltipUI.Hide();
                return;
            }

            bool isActive = stopUI.activeSelf;
            stopUI.SetActive(!isActive);
        }
    }
    public void UpdateHpSmooth(float targetHp, float maxHp)
    {
        StartCoroutine(SmoothHpBar(targetHp, maxHp));
    }

    IEnumerator SmoothHpBar(float targetHp, float maxHp)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        float startValue = HpBarSlider.value;
        float targetValue = (float)targetHp / maxHp;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            HpBarSlider.value = currentValue;
            hpTxt.text = $"{(int)(currentValue * maxHp)}/{(int)maxHp}";
            yield return null;
        }

        HpBarSlider.value = targetValue;
        hpTxt.text = $"{(int)targetHp}/{(int)maxHp}";
    }
    public void CheckGold()
    {
        if (playerStatus != null && goldText != null)
        {
            goldText.text = playerStatus.Gold.ToString();
        }
    }


    private void Damage(float damage)
    {
        if (playerStatus.MaxHp == 0 || playerStatus.Hp <= 0) //* 이미 체력 0이하면 패스
            return;
        playerStatus.Hp -= damage;
        UpdateHpSmooth(playerStatus.Hp,playerStatus.MaxHp); //* 체력 갱신
        if (playerStatus.Hp <= 0)
        {
            //사망
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
