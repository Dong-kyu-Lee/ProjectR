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

    [SerializeField] private Transform statusContent;

    List<GameObject> statusObjList = new List<GameObject>();

    private Inventory cachedInventory;

    private void Awake()
    {
        if (characterInfo == null)
        {
            Debug.LogWarning("CharacterInfo 오브젝트가 Inspector에 할당되지 않았습니다!");
        }

        // 닫기 버튼 리스너 등록 및 초기 비활성화는 플레이어 생성 여부와 무관하게 항상 실행
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(DisableUI);
        }

        DisableUI();

        // 플레이어 캐릭터가 변경되거나 최초 스폰될 때 연동되도록 이벤트 구독
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerCharacterChanged.RemoveListener(LinkPlayerAndUI);
            PlayerManager.Instance.OnPlayerCharacterChanged.AddListener(LinkPlayerAndUI);
        }

        // 만약 Awake 시점에 이미 플레이어가 씬에 로드되어 있다면 즉시 연동
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            LinkPlayerAndUI();
        }
    }

    private void OnEnable()
    {
        // 플레이어가 있을 때만 초기화 진행
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null)
        {
            Init();
            SetStatus();
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

    // UI 활성화
    public void EnableUI()
    {
        RefreshStatusUI();
        if (characterInfo != null) characterInfo.SetActive(true);
    }

    // UI 비활성화
    public void DisableUI()
    {
        if (characterInfo != null) characterInfo.SetActive(false);
    }

    // 플레이어가 준비되었을 때 UI 컴포넌트와 플레이어 데이터를 직접 연결하는 핵심 함수
    private void LinkPlayerAndUI()
    {
        Init();
        SetStatus();

        // 자식이 비활성화 상태여도 탐색할 수 있도록 true 인자값 추가
        InventoryUI invUI = transform.GetComponentInChildren<InventoryUI>(true);
        if (invUI != null)
        {
            invUI.Init();
        }
    }

    private void RefreshStatusUI()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null) return;

        InitPlayerStatus();   // PlayerStatus 다시 찾기
        ClearStatusTexts();   // 기존 텍스트 오브젝트 삭제
        SetStatus();          // 최신 값으로 다시 생성
    }

    private void InitPlayerStatus()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null)
            return;

        var ps = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        if (ps != null)
            playerStatus = ps;

        if (cachedInventory != null)
        {
            cachedInventory.OnStatusChanged -= RefreshStatusUI;
        }

        cachedInventory = GameManager.Instance.CurrentPlayer.GetComponentInChildren<Inventory>();

        // 이벤트 연결
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

        if (inventoryUI == null)
        {
            return;
        }

        GameObject panelRoot = characterInfo;
        if (panelRoot != null)
        {
            if (panelRoot.activeSelf)
            {
                // 닫기
                DisableUI();

                if (controller != null)
                    controller.DisableCharacterUI();
            }
            else
            {
                // 열기
                EnableUI();

                if (controller != null && hasInventoryEvent)
                    controller.OnEnableCharacterInfoUI.Invoke();
            }
        }
    }

    // 세팅 전 초기화
    void Init()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null) return;

        // 기존 오브젝트 삭제
        foreach (var obj in statusObjList)
        {
            if (obj != null) Destroy(obj);
        }
        statusObjList.Clear();

        // PlayerStatus 다시 가져오기
        if (playerStatus == null || playerStatus.gameObject != GameManager.Instance.CurrentPlayer)
        {
            playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
            if (playerStatus == null)
                Debug.Log("PlayerStatus 없음");
        }
    }

    // 스테이터스 세팅
    void SetStatus()
    {
        // 플레이어 없으면 리턴
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

        // 캐릭터 이름 처리
        if (characterNameText != null)
        {
            string playerName = GameManager.Instance?.CurrentPlayer?.GetComponent<PlayerControllerBase>()?.playerName;
            if (characterNameText.TryGetComponent<Text>(out var nameText))
            {
                switch (playerName)
                {
                    case "bartender":
                        nameText.text = "바텐더";
                        break;
                    case "blacksmith":
                        nameText.text = "대장장이";
                        break;
                }
            }
        }

        // ====== 실제 스탯 표시 ======
        float additionalDamageValue =
            Mathf.Round(playerStatus.Damage * playerStatus.AdditionalDamage * 100f) / 100f;

        AddStatusLine($"레벨 : {playerStatus.Level}");
        AddStatusLine($"체력 : {playerStatus.Hp} / {playerStatus.MaxHp}");
        AddStatusLine($"경험치 : {playerStatus.Exp} / {LevelUp.requiredExp[(int)playerStatus.Level]}");
        AddStatusLine(
            $"피해량 : {playerStatus.TotalDamage}(" +
            $"{playerStatus.Damage}+" +
            $"<color=yellow>{additionalDamageValue}</color>" +
            $"<color=black>)</color>"
        );
        AddStatusLine($"추가 피해량 : {Mathf.Round(playerStatus.AdditionalDamage * 100f)}%");
        AddStatusLine($"치명타 확률 : {Mathf.Round(playerStatus.CriticalPercent * 100f)}%");
        AddStatusLine($"치명타 피해량 : {Mathf.Round(playerStatus.CriticalDamage * 100f)}%");
        AddStatusLine($"피해 감소량 : {Mathf.Round(playerStatus.DamageReduction * 100f)}%");
        AddStatusLine($"피해 감소량 무시 : {Mathf.Round(playerStatus.IgnoreDamageReduction * 100f)}%");
        AddStatusLine($"공격속도 : {Mathf.Round(playerStatus.TotalAttackSpeed * 100) / 100}");
        AddStatusLine($"이동속도 : {100 + Mathf.Round(playerStatus.AdditionalMoveSpeed * 100f)}%");
        AddStatusLine($"버프 지속시간 : {Mathf.Round(playerStatus.BuffDuration * 100f)}%");
        AddStatusLine($"디버프 피해량 : {Mathf.Round(playerStatus.DebuffDamage * 100f)}%");
        AddStatusLine($"재화 획득량 : {Mathf.Round(playerStatus.PriceAdditional * 100f)}%");
    }

    private void AddStatusLine(string text)
    {
        if (statusContent == null || statusTextPref == null) return;

        var go = Instantiate(statusTextPref, statusContent);
        var tmp = go.GetComponent<Text>();
        if (tmp != null)
            tmp.text = text;

        statusObjList.Add(go);
    }
}