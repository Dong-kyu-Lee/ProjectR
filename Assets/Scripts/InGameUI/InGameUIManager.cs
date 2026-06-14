using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public CharacterUIManager CharacterUI;
    public CharacterInfo characterInfoUI;
    public GameSettingUI gameSettingUI;
    [SerializeField] private UpgradeUI upgradeUI;

    public SkillCoolTime skillCoolTimeUI;

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

        if (gameSettingUI == null) gameSettingUI = FindObjectOfType<GameSettingUI>(true);
        if (CharacterUI == null) CharacterUI = FindObjectOfType<CharacterUIManager>(true);
        if (checkUI != null) checkUI.SetActive(false);
    }

    private void Start()
    {
        if (upgradeUI == null) upgradeUI = FindObjectOfType<UpgradeUI>(true);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "EndScene") return;

        // 인벤토리 (I 키)
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (characterInfoUI != null) characterInfoUI.ToggleInventoryUI();
        }

        // 업그레이드 (O 키)
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
        // GameManager의 새로운 씬 변경 이벤트 구독
        if (GameManager.Instance != null) GameManager.Instance.OnSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(SceneType sceneType)
    {
        if (characterInfoUI != null) characterInfoUI.gameObject.SetActive(false);

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
    // [Wrapper 함수들] 코드가 깨지지 않도록 연결해주는 역할만 수행
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
}