using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
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
    private Canvas rootCanvas;

    [SerializeField] private GameObject checkUI;
    [SerializeField] private Text goldText;
    [SerializeField] private Slider HpBarSlider;
    [SerializeField] private Text hpTxt;
    [SerializeField] private BuffToolTipUI tooltipUI;
    [SerializeField] private Image PlayerHead;
    [SerializeField] private CharacterUIManager CharacterUI;
    [SerializeField] private UpgradeUI upgradeUI;
    [SerializeField] private TextMeshProUGUI warpUIText;

    // 레벨 및 경험치 UI 연결 변수
    [SerializeField] private Slider expBar;
    [SerializeField] private Text levelText; // 레벨 표시용 텍스트
    [SerializeField] private Text expText;

    public SkillCoolTime skillCoolTimeUI;
    public PlayerStatus playerStatus;

    public CharacterInfo characterInfoUI;
    public GameSettingUI gameSettingUI;

    //열린 UI들을 관리하는 스택 (최근 열린 순서대로 저장)
    private Stack<GameObject> uiStack = new Stack<GameObject>();

    // 워프에 닿았을 때 E 키를 누르면 실행할 함수
    private Action warpAction;

    // UIConnector가 호출하여 CharacterInfo를 연결해주는 함수
    public void SetCharacterInfoUI(CharacterInfo info)
    {
        this.characterInfoUI = info;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else if (instance == this)
        {
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else
        {
            Destroy(this.transform.root.gameObject);
            return;
        }

        rootCanvas = GetComponentInParent<Canvas>();

        if (gameSettingUI == null)
        {
            gameSettingUI = FindObjectOfType<GameSettingUI>(true);
            if (gameSettingUI == null) Debug.LogWarning("GameSettingUI를 찾을 수 없습니다.");
        }

        if (checkUI != null)
            checkUI.SetActive(false);

        if (CharacterUI == null)
        {
            CharacterUI = FindObjectOfType<CharacterUIManager>();
        }
    }

    private void Start()
    {
        if (GameManager.Instance.CurrentPlayer != null)
        {
            OnPlayerChanged();
        }

        if (upgradeUI == null)
        {
            upgradeUI = FindObjectOfType<UpgradeUI>(true);
            if (upgradeUI == null) Debug.LogWarning("UpgradeUI를 찾을 수 없습니다.");
        }

        if (gameSettingUI == null) gameSettingUI = FindObjectOfType<GameSettingUI>();
    }

    // 플레이어가 변경/생성되었을 때 실행되는 함수
    public void OnPlayerChanged()
    {
        // 기존 코루틴 중단 후 재시작 (중복 실행 방지)
        StopAllCoroutines();
        StartCoroutine(InitPlayerStatus());
    }

    private IEnumerator InitPlayerStatus()
    {
        // 플레이어가 확실히 준비될 때까지 대기
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null);

        // PlayerStatus 컴포넌트 찾기
        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        while (playerStatus == null)
        {
            yield return null; // 없으면 다음 프레임까지 대기
            if (GameManager.Instance.CurrentPlayer != null)
                playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        }

        // UI 연결 대기
        float timeOut = 3.0f; // 최대 3초 대기
        while (characterInfoUI == null && timeOut > 0)
        {
            timeOut -= Time.deltaTime;
            yield return null;
        }

        if (characterInfoUI != null)
        {
            characterInfoUI.EnableUI();

            if (CharacterUI != null)
            {
                CharacterUI.InitUIForCurrentPlayer();
            }
            else
            {
                Debug.LogError("CharacterUIManager가 연결되지 않음. Inspector를 확인하세요.");
            }

            characterInfoUI.DisableUI();

            InventoryUI invUI = characterInfoUI.GetComponentInChildren<InventoryUI>(true);
            if (invUI != null) invUI.Init();
        }
        else
        {
            Debug.LogWarning("CharacterInfo가 연결되지 않음");
        }

        CheckGold();
        UpdateHpSmooth(playerStatus.Hp, playerStatus.MaxHp);

        // 초기 레벨 및 경험치 UI 갱신
        if (playerStatus != null)
        {
            int currentLevel = (int)playerStatus.Level;
            int maxExp = 100;
            if (LevelUp.requiredExp != null && LevelUp.requiredExp.Length > currentLevel)
            {
                maxExp = LevelUp.requiredExp[currentLevel];
            }

            UpdateLevelUI(currentLevel);
            UpdateExpUI(playerStatus.Exp, maxExp);
        }
    }

    private void Update()
    {
        //ESC 입력 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeInput();
        }

        // 인벤토리 (I 키)
        if (Input.GetKeyDown(KeyCode.I))
        {

            if (characterInfoUI != null)
            {
                characterInfoUI.ToggleInventoryUI();
            }
        }

        // 워프 상호작용 (E 키)
        if (Input.GetKeyDown(KeyCode.E) && warpUIText.IsActive())
        {
            warpAction?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            upgradeUI.SetActiveUI();
        }
    }

    // ESC 키 로직 분리
    private void HandleEscapeInput()
    {
        // 스택에 쌓인 UI가 있다면 닫기
        if (uiStack.Count > 0)
        {
            GameObject topUI = uiStack.Peek();

            DungeonUIManager dungeonManager = FindObjectOfType<DungeonUIManager>();

            if (dungeonManager != null && topUI == dungeonManager.fullMap)
            {
                dungeonManager.CloseFullMap();
            }
            // GameSettingUI 처리
            else if (topUI.GetComponentInParent<GameSettingUI>() != null)
            {
                topUI.GetComponentInParent<GameSettingUI>().OpenCloseSettingUI();
            }
            // 그 외 일반 UI
            else
            {
                topUI.SetActive(false);
                if (uiStack.Count > 0 && uiStack.Peek() == topUI) uiStack.Pop();
            }
            return;
        }

        // 아무 창도 없으면 설정창 열기
        if (gameSettingUI != null)
        {
            gameSettingUI.OpenCloseSettingUI();
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

        if (uiStack.Count > 0 && uiStack.Peek() == ui)
        {
            uiStack.Pop();
        }
        else
        {
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

    // 경험치 UI 업데이트
    public void UpdateExpUI(float currentExp, float maxExp)
    {
        if (expBar != null)
        {
            expBar.value = (maxExp > 0) ? (currentExp / maxExp) : 0;
        }

        if (expText != null)
        {
            expText.text = $"{(int)currentExp} / {(int)maxExp}";
        }
    }

    // 레벨 텍스트 업데이트
    public void UpdateLevelUI(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Lv." + level;
        }
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

    public void FirstToLobby()
    {
        checkUI.SetActive(true);
    }

    public void ToLobby()
    {
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

    // 워프에 닿았을 때 UI 표시
    // message : 표시할 메시지
    // action : E 키를 눌렀을 때 실행할 함수
    public void ShowWarpUI(string message, Action action)
    {
        if (warpUIText != null)
        {
            warpUIText.text = message;
            warpAction = action;
            warpUIText.gameObject.SetActive(true);
        }
    }

    public void HideWarpUI()
    {
        warpUIText.gameObject.SetActive(false);
        warpAction = null;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerCharacterChanged.AddListener(OnPlayerChanged);
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerCharacterChanged.RemoveListener(OnPlayerChanged);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (rootCanvas == null) return;

        if (scene.name == "EndScene")
        {
            rootCanvas.enabled = false;
        }
        else
        {
            rootCanvas.enabled = true;
        }
    }
}