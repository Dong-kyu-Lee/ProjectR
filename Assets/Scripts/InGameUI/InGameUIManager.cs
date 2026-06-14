using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
                if (instance == null) Debug.LogError("InGameUIManager가 씬에 존재하지 않습니다.");
            }
            return instance;
        }
    }

    private Canvas rootCanvas;

    [Header("UI References")]
    [SerializeField] private GameObject checkUI;

    // UI 필드들
    [SerializeField] private Text goldText;
    [SerializeField] private Slider HpBarSlider;
    [SerializeField] private Text hpTxt;
    [SerializeField] private BuffToolTipUI tooltipUI;
    [SerializeField] private Image PlayerHead;
    [SerializeField] private Sprite blacksmithHeadSprite;
    [SerializeField] private Sprite bartenderHeadSprite;
    [SerializeField] private UpgradeUI upgradeUI;
    [SerializeField] private TextMeshProUGUI warpUIText;

    // 레벨 및 경험치 UI
    [SerializeField] private Slider expBar;
    [SerializeField] private Text levelText; 
    [SerializeField] private Text expText;

    private MessageManager messageManager;

    public SkillCoolTime skillCoolTimeUI;
    public PlayerStatus playerStatus;
    public CharacterUIManager CharacterUI;
    public CharacterInfo characterInfoUI;
    public GameSettingUI gameSettingUI;

    public bool IsUIActive
    {
        get
        {
            if (UIStackManager.Instance != null) return UIStackManager.Instance.IsUIActive;
            return false;
        }
    }

    public void SetCharacterInfoUI(CharacterInfo info)
    {
        this.characterInfoUI = info;
        if (characterInfoUI != null)
        {
            characterInfoUI.EnableUI();
            if (CharacterUI != null) CharacterUI.InitUIForCurrentPlayer();
            characterInfoUI.DisableUI();

            InventoryUI invUI = characterInfoUI.GetComponentInChildren<InventoryUI>(true);
            if (invUI != null) invUI.Init();
        }
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

        if (checkUI != null) checkUI.SetActive(false);

        if (CharacterUI == null)
        {
            CharacterUI = FindObjectOfType<CharacterUIManager>();
        }

        messageManager = GetComponent<MessageManager>();
    }

    private void Start()
    {
        if (upgradeUI == null) upgradeUI = FindObjectOfType<UpgradeUI>(true);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "EndScene") return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (characterInfoUI != null) characterInfoUI.ToggleInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (upgradeUI != null) upgradeUI.SetActiveUI();
            else upgradeUI = FindObjectOfType<UpgradeUI>(true);
        }
    }

    public void FirstToLobby() { checkUI.SetActive(true); }
    public void ToLobby() { checkUI.SetActive(false); GameManager.Instance.MoveScene(SceneType.LobbyScene, "Lobby + UpgradeScene"); }
    public void ExitButton() { Application.Quit(); }
    public void CancelButton() { checkUI.SetActive(false); }

    private void OnEnable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(SceneType sceneType)
    {
        if (characterInfoUI != null)
            characterInfoUI.gameObject.SetActive(false);

        if (gameSettingUI != null)
        {
            Transform menuContainer = gameSettingUI.transform.Find("MenuContainer");
            if (menuContainer != null) menuContainer.gameObject.SetActive(false);
        }

        if (upgradeUI != null)
        {
            Transform upgradeStatus = upgradeUI.transform.Find("UpgradeStatusUI");
            if (upgradeStatus != null) upgradeStatus.gameObject.SetActive(false);
        }

        if (UIStackManager.Instance != null) UIStackManager.Instance.ClearStack();

        if (sceneType == SceneType.EndScene)
        {
            if (rootCanvas != null) rootCanvas.enabled = false;
        }
        else
        {
            if (rootCanvas != null) rootCanvas.enabled = true;
        }
    }

    // =====================================================================
    // [Wrapper 함수들] 기존 로직 유지 및 신규 기능 통합
    // =====================================================================

    // 1. PlayerStatusUI 연결 다리
    public void CheckGold() { if (PlayerStatusUI.Instance != null) PlayerStatusUI.Instance.CheckGold(); }
    public void UpdateHpSmooth(float targetHp, float maxHp) { if (PlayerStatusUI.Instance != null) PlayerStatusUI.Instance.UpdateHpSmooth(targetHp, maxHp); }
    public void UpdateExpUI(float currentExp, float maxExp) { if (PlayerStatusUI.Instance != null) PlayerStatusUI.Instance.UpdateExpUI(currentExp, maxExp); }
    public void UpdateLevelUI(int level) { if (PlayerStatusUI.Instance != null) PlayerStatusUI.Instance.UpdateLevelUI(level); }

    // 2. UIStackManager 연결 다리
    public void RegisterUI(GameObject ui) { if (UIStackManager.Instance != null) UIStackManager.Instance.RegisterUI(ui); }
    public void UnregisterUI(GameObject ui) { if (UIStackManager.Instance != null) UIStackManager.Instance.UnregisterUI(ui); }

    // 3. InteractionUI 연결 다리
    public void ShowWarpUI(string message, Action action) { if (InteractionUI.Instance != null) InteractionUI.Instance.ShowWarpUI(message, action); }
    public void HideWarpUI() { if (InteractionUI.Instance != null) InteractionUI.Instance.HideWarpUI(); }

    // 4. 메시지 시스템 연결 다리
    public void ShowStatus(string msg, float delay = 2f)
    {
        if (messageManager != null)
        {
            messageManager.ShowMessage(msg, delay);
        }
    }
}