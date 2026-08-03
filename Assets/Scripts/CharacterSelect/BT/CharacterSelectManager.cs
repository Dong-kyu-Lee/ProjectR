using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager Instance { get; private set; }

    public bool IsSelectionMode { get; private set; } = true;

    [Header("Camera & Spawn Settings")]
    [SerializeField] private CM_LobbyScene vcam;
    [SerializeField] private Transform defaultCameraTarget; // 전체 캐릭터가 보이는 로비 중앙 위치
    [SerializeField] private Transform prologueSpawnPoint;
    [SerializeField] private Transform defaultLobbySpawnPoint; // 기존 마네킹들이 서 있던 로비 시작 위치

    [Header("Lobby Characters")]
    [SerializeField] private GameObject[] selectableCharacterObjects; // 로비에 서 있는 대기용 캐릭터 프리팹들

    // [SerializeField] private CharacterPortraitHandler portraitHandler; (필요한 경우 추후 연결)

    private CharacterType currentPreviewType; // 현재 화면에 확대된 캐릭터 타입

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 씬 시작 시 마우스 기반 캐릭터 선택 모드로 진입
        EnterSelectionMode();
    }

    // 마우스 클릭 시 호출됨 (줌인 및 상세 정보 띄우기)
    public void SelectCharacter(CharacterType type, Transform characterTransform)
    {
        if (!IsSelectionMode) return;

        currentPreviewType = type;

        // 1. 카메라 타겟을 클릭한 캐릭터로 변경
        vcam.SetFollowTarget(characterTransform);

        vcam.SetZoom(true); 
        CharacterSelectUI.Instance.ShowDetailPanels(type); 
    }

    // [선택하기] 버튼 클릭 시 호출
    public void ConfirmSelection()
    {
        IsSelectionMode = false;

        // 1. 스폰 위치 결정
        Vector3 spawnPosition;
        if (PlayerPrefs.GetInt("HasSeenPrologue") == 0)
        {
            spawnPosition = prologueSpawnPoint.position;
            PlayerPrefs.SetInt("HasSeenPrologue", 1);
            PlayerPrefs.Save();
        }
        else
        {
            spawnPosition = defaultLobbySpawnPoint != null ? defaultLobbySpawnPoint.position : Vector3.zero;
        }

        // 2. PlayerManager를 통해 실제 조작할 플레이어 생성 및 배치
        GameObject characterInstance = Instantiate(PlayerManager.Instance.GetCharacterPrefab(currentPreviewType), Vector3.zero, Quaternion.identity);
        PlayerManager.Instance.SetCurrentPlayer(characterInstance, currentPreviewType, spawnPosition);
        PlayerManager.Instance.CurrentPlayer.SetActive(true);

        // 3. 로비에 서 있던 대기용 캐릭터들 숨기기
        foreach (var obj in selectableCharacterObjects)
        {
            if (obj != null)
            {
                SelectableCharacter selectable = obj.GetComponent<SelectableCharacter>();
                // 선택한 캐릭터의 원본(마네킹)만 숨겨서 자리를 비우고, 나머지는 켜둠
                if (selectable != null && selectable.characterType == currentPreviewType)
                {
                    obj.SetActive(false);
                }
                else
                {
                    obj.SetActive(true);
                }
            }
        }
        // 4. 카메라를 조작할 플레이어로 연결
        vcam.SetFollowTarget(PlayerManager.Instance.CurrentPlayer.transform);


        vcam.SetZoom(false);
        CharacterSelectUI.Instance.HideDetailPanels();
    }

    // [X] (취소) 버튼 누를 시 호출
    public void CancelSelection()
    {
        vcam.SetFollowTarget(defaultCameraTarget);


        vcam.SetZoom(false);
        CharacterSelectUI.Instance.HideDetailPanels();
    }

    // 'E' 키를 눌러 캐릭터 선택 창으로 복귀할 때 호출
    public void EnterSelectionMode()
    {
        IsSelectionMode = true;

        // 1. 기존 조작하던 플레이어 캐릭터 비활성화
        if (PlayerManager.Instance.CurrentPlayer != null)
        {
            PlayerManager.Instance.CurrentPlayer.SetActive(false);
        }

        // 2. 로비 대기용 캐릭터들 다시 등장
        foreach (var obj in selectableCharacterObjects)
        {
            if (obj != null) obj.SetActive(true);
        }

        // 3. 카메라를 초기 중앙 위치로 이동
        if (defaultCameraTarget != null)
        {
            vcam.SetFollowTarget(defaultCameraTarget);
        }

        vcam.SetZoom(false);
    }
}