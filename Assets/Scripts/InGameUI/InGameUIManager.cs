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

    [SerializeField] private GameObject stopUI;
    [SerializeField] private GameObject checkUI;
    [SerializeField] private Text goldText;
    [SerializeField] private Slider HpBarSlider;
    [SerializeField] private Text hpTxt;
    [SerializeField] private BuffToolTipUI tooltipUI;
    [SerializeField] private Image PlayerHead;
    [SerializeField] private CharacterUIManager CharacterUI;

    public PlayerStatus playerStatus;

    //열린 UI들을 관리하는 스택 (최근 열린 순서대로 저장)
    private Stack<GameObject> uiStack = new Stack<GameObject>();

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
            //버프 툴팁이 켜져 있으면 닫기
            if (tooltipUI != null && tooltipUI.IsVisible())
            {
                tooltipUI.Hide();
                return;
            }

            //열려 있는 UI가 있다면 최근 UI부터 닫기
            if (uiStack.Count > 0)
            {
                GameObject topUI = uiStack.Pop();
                if (topUI != null)
                    topUI.SetActive(false);
                return;
            }

            //열린 UI가 하나도 없으면 정지 메뉴 토글
            bool isActive = stopUI.activeSelf;
            stopUI.SetActive(!isActive);
        }
    }

    //UI 열릴 때 호출
    public void RegisterUI(GameObject ui)
    {
        if (ui == null) return;

        //이미 스택에 있는 경우 중복 등록 방지
        if (!uiStack.Contains(ui))
        {
            uiStack.Push(ui);
        }
    }

    //UI 닫힐 때 호출
    public void UnregisterUI(GameObject ui)
    {
        if (ui == null) return;

        //스택 최상단이라면 Pop()
        if (uiStack.Count > 0 && uiStack.Peek() == ui)
        {
            uiStack.Pop();
        }
        else
        {
            //중간에 있는 경우 제거 후 재정렬
            Stack<GameObject> tempStack = new Stack<GameObject>(uiStack);
            uiStack.Clear();
            foreach (GameObject go in tempStack)
            {
                if (go != ui)
                    uiStack.Push(go);
            }
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
        if (playerStatus.MaxHp == 0 || playerStatus.Hp <= 0)
            return;

        playerStatus.Hp -= damage;
        UpdateHpSmooth(playerStatus.Hp, playerStatus.MaxHp);

        if (playerStatus.Hp <= 0)
        {
            //사망 처리
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
