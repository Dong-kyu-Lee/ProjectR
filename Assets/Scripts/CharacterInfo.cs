using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public GameObject characterNameText;
    public GameObject statusTextPref;
    public GameObject statusParentObj;
    [SerializeField]
    public GameObject characterInfo;
    public PlayerStatus playerStatus;
    public Button closeButton;

    // 2열 배치를 위해 부모 Transform을 2개로 나눕니다.
    [Header("스탯 UI 배치")]
    [SerializeField] private Transform leftStatusContent;  // 1열: 기본 스탯
    [SerializeField] private Transform rightStatusContent; // 2열: 치명타 등 세부 스탯

    [Header("스탯 포인트 UI")]
    [SerializeField] private Button enhanceButton;
    private UpgradeStatus upgradeStatus;

    List<GameObject> statusObjList = new List<GameObject>();

    private Inventory cachedInventory;

    [Header("캐릭터 이미지 UI")]
    [SerializeField] private Image centerCharacterImage;

    private void Awake()
    {
        if (characterInfo == null)
        {
            Debug.LogWarning("CharacterInfo 오브젝트가 Inspector에 할당되지 않았습니다!");
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(DisableUI);
        }

        // 강화 버튼 이벤트 리스너 등록
        if (enhanceButton != null)
        {
            enhanceButton.onClick.RemoveAllListeners();
            enhanceButton.onClick.AddListener(OnClickEnhanceButton);
        }

        DisableUI();

        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerCharacterChanged.RemoveListener(LinkPlayerAndUI);
            PlayerManager.Instance.OnPlayerCharacterChanged.AddListener(LinkPlayerAndUI);
        }

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            LinkPlayerAndUI();
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            Init();
            SetStatus();
            UpdateCharacterImage();
        }
    }

    private void UpdateCharacterImage()
    {
        if (PlayerManager.Instance == null) return;
        CharacterType currentType = PlayerManager.Instance.CurrentCharacterType;
        CharacterData data = PlayerManager.Instance.GetCharacterData(currentType);

        if (centerCharacterImage != null && data.inventoryImage != null)
        {
            centerCharacterImage.sprite = data.inventoryImage;
        }
    }

    private void OnDisable()
    {
        if (cachedInventory != null)
        {
            cachedInventory.OnStatusChanged -= RefreshStatusUI;
        }
        if (characterInfo != null) characterInfo.SetActive(false);
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerCharacterChanged.RemoveListener(LinkPlayerAndUI);
        }
    }

    public void EnableUI()
    {
        RefreshStatusUI();
        if (characterInfo != null) characterInfo.SetActive(true);
    }

    public void DisableUI()
    {
        if (characterInfo != null) characterInfo.SetActive(false);
    }

    private void LinkPlayerAndUI()
    {
        Init();
        SetStatus();

        InventoryUI invUI = transform.GetComponentInChildren<InventoryUI>(true);
        if (invUI != null)
        {
            invUI.Init();
        }
    }

    public void RefreshStatusUI()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null) return;

        InitPlayerStatus();
        ClearStatusTexts();
        SetStatus();
    }

    private void InitPlayerStatus()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null)
            return;

        var ps = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        if (ps != null)
            playerStatus = ps;

        // UpgradeStatus 컴포넌트 캐싱
        upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();

        if (cachedInventory != null)
        {
            cachedInventory.OnStatusChanged -= RefreshStatusUI;
        }

        cachedInventory = GameManager.Instance.CurrentPlayer.GetComponentInChildren<Inventory>();

        if (cachedInventory != null)
        {
            cachedInventory.OnStatusChanged += RefreshStatusUI;
        }
    }

    private void ClearStatusTexts()
    {
        foreach (var obj in statusObjList)
        {
            if (obj != null)
                Destroy(obj);
        }
        statusObjList.Clear();
    }

    public void ToggleInventoryUI()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null) return;

        BartenderController controller = GameManager.Instance.CurrentPlayer.GetComponent<BartenderController>();
        bool hasInventoryEvent = controller != null &&
            controller.OnEnableCharacterInfoUI != null &&
            controller.OnEnableCharacterInfoUI.GetPersistentEventCount() > 0;

        InventoryUI inventoryUI = GetComponentInChildren<InventoryUI>(true);

        if (inventoryUI == null) return;

        GameObject panelRoot = characterInfo;
        if (panelRoot != null)
        {
            if (panelRoot.activeSelf)
            {
                DisableUI();
                if (controller != null) controller.DisableCharacterUI();
            }
            else
            {
                EnableUI();
                if (controller != null && hasInventoryEvent) controller.OnEnableCharacterInfoUI.Invoke();
            }
        }
    }

    void Init()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null) return;

        foreach (var obj in statusObjList)
        {
            if (obj != null) Destroy(obj);
        }
        statusObjList.Clear();

        if (playerStatus == null || playerStatus.gameObject != GameManager.Instance.CurrentPlayer)
        {
            playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
            if (playerStatus == null) Debug.Log("PlayerStatus 없음");
        }

        // 초기화 시 UpgradeStatus 확보 보장
        if (upgradeStatus == null || upgradeStatus.gameObject != GameManager.Instance.CurrentPlayer)
        {
            upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
        }
    }

    void SetStatus()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null) return;

        if (playerStatus == null)
        {
            playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
            if (playerStatus == null)
            {
                Debug.LogWarning("CharacterInfo: PlayerStatus를 찾을 수 없습니다.");
                return;
            }
        }

        if (characterNameText != null)
        {
            string playerName = GameManager.Instance?.CurrentPlayer?.GetComponent<PlayerControllerBase>()?.playerName;
            if (characterNameText.TryGetComponent<Text>(out var nameText))
            {
                switch (playerName)
                {
                    case "bartender": nameText.text = "바텐더"; break;
                    case "blacksmith": nameText.text = "대장장이"; break;
                }
            }
        }

        float additionalDamageValue = Mathf.Round(playerStatus.Damage * playerStatus.AdditionalDamage * 100f) / 100f;

        // 1열: 기본 스테이터스
        AddStatusLine($"레벨 : {playerStatus.Level}", leftStatusContent);
        AddStatusLine($"체력 : {playerStatus.Hp} / {playerStatus.MaxHp}", leftStatusContent);
        AddStatusLine($"경험치 : {playerStatus.Exp} / {LevelUp.requiredExp[(int)playerStatus.Level]}", leftStatusContent);
        AddStatusLine($"피해량 : {playerStatus.TotalDamage}({playerStatus.Damage}+<color=yellow>{additionalDamageValue}</color><color=black>)</color>", leftStatusContent);
        AddStatusLine($"공격속도 : {Mathf.Round(playerStatus.TotalAttackSpeed * 100) / 100}", leftStatusContent);
        AddStatusLine($"이동속도 : {100 + Mathf.Round(playerStatus.AdditionalMoveSpeed * 100f)}%", leftStatusContent);
        if (upgradeStatus != null)
        {
            int sp = upgradeStatus.StatPoint;
            string colorHex = sp > 0 ? "red" : "black";
            AddStatusLine($"남은 스탯포인트 : <color={colorHex}>{sp}</color>", leftStatusContent);
        }

        // 2열: 세부 스테이터스
        AddStatusLine($"추가 피해량 : {Mathf.Round(playerStatus.AdditionalDamage * 100f)}%", rightStatusContent);
        AddStatusLine($"치명타 확률 : {Mathf.Round(playerStatus.CriticalPercent * 100f)}%", rightStatusContent);
        AddStatusLine($"치명타 피해량 : {Mathf.Round(playerStatus.CriticalDamage * 100f)}%", rightStatusContent);
        AddStatusLine($"피해 감소량 : {Mathf.Round(playerStatus.DamageReduction * 100f)}%", rightStatusContent);
        AddStatusLine($"피해 감소량 무시 : {Mathf.Round(playerStatus.IgnoreDamageReduction * 100f)}%", rightStatusContent);
        AddStatusLine($"버프 지속시간 : {Mathf.Round(playerStatus.BuffDuration * 100f)}%", rightStatusContent);
        AddStatusLine($"디버프 피해량 : {Mathf.Round(playerStatus.DebuffDamage * 100f)}%", rightStatusContent);
        AddStatusLine($"재화 획득량 : {Mathf.Round(playerStatus.PriceAdditional * 100f)}%", rightStatusContent);
    }

    private void AddStatusLine(string text, Transform parentContent)
    {
        if (parentContent == null || statusTextPref == null) return;

        var go = Instantiate(statusTextPref, parentContent);
        var tmp = go.GetComponent<Text>();
        if (tmp != null) tmp.text = text;

        statusObjList.Add(go);
    }

    // 강화 버튼 클릭 시 실행될 메서드
    private void OnClickEnhanceButton()
    {
        // 1. 인벤토리 완전히 닫기
        if (characterInfo != null && characterInfo.activeSelf)
        {
            ToggleInventoryUI();
        }

        // 2. 스탯 투자 창 열기
        if (UpgradeUI.Instance != null)
        {
            UpgradeUI.Instance.OpenUI();
        }
    }
}