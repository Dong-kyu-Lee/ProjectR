using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public GameObject characterName;
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

        // 플레이어가 없으면 초기화 중단
        if (GameManager.Instance == null || GameManager.Instance.CurrentPlayer == null)
        {
            return;
        }

        Init();
        SetStatus();

        // InventoryUI 초기화 안전장치
        InventoryUI invUI = transform.GetComponentInChildren<InventoryUI>();
        if (invUI != null) invUI.Init();

        if (closeButton != null)
            closeButton.onClick.AddListener(DisableUI);

        DisableUI();
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

        // RefreshStatusUI / Init 에서 이미 playerStatus 세팅 시도하니까
        // 여기서는 한 번만 더 확인하고, 없으면 그냥 리턴
        if (playerStatus == null)
        {
            playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();

            if (playerStatus == null)
            {
                Debug.LogWarning("CharacterInfo: PlayerStatus를 찾을 수 없습니다.");
                return;
            }
        }

        // 캐릭터 이름 (나중에 캐릭터별로 분기하고 싶으면 여기서 처리)
        if (characterName != null)
        {
            var nameText = characterName.GetComponent<Text>();
            if (nameText != null)
                nameText.text = "바텐더";
        }

        // ====== 실제 스탯 표시 ======
        float additionalDamageValue =
            playerStatus.Damage * playerStatus.AdditionalDamage * 0.01f;
        float additionalDamageReductionValue =
            playerStatus.DamageReduction * playerStatus.AdditionalDamageReduction * 0.01f;

        // 필요하면 경험치/HP 라인도 추가 가능
        AddStatusLine($"레벨 : {playerStatus.Level}");
        //AddStatusLine($"경험치 : {playerStatus.Exp} / {playerStatus.NeededExp}");
        AddStatusLine(
            $"피해량 : {playerStatus.TotalDamage}(" +
            $"{playerStatus.Damage}+" +
            $"<color=yellow>{additionalDamageValue}</color>" +
            $"<color=black>)</color>"
        );
        AddStatusLine($"추가 피해량 : {playerStatus.AdditionalDamage}%");
        AddStatusLine($"치명타 확률 : {playerStatus.CriticalPercent}%");
        AddStatusLine(
            $"피해 감소량 : {playerStatus.TotalDamageReduction}(" +
            $"{playerStatus.DamageReduction}+" +
            $"<color=yellow>{additionalDamageReductionValue}</color>" +
            $"<color=black>)</color>"
        );
        AddStatusLine($"추가 피해 감소량 : {playerStatus.AdditionalDamageReduction}%");
        AddStatusLine($"공격속도 : {playerStatus.AttackSpeed}");
        AddStatusLine($"이동속도 : {playerStatus.MoveSpeed}");
        AddStatusLine($"재화 획득량 : {playerStatus.PriceAdditional}");
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