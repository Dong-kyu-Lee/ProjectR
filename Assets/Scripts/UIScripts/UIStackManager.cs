using System.Collections.Generic;
using UnityEngine;

public class UIStackManager : MonoBehaviour
{
    private static UIStackManager instance;
    public static UIStackManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UIStackManager>();
            return instance;
        }
    }

    // 열린 UI들을 관리하는 스택 (최근 열린 순서대로 저장)
    private Stack<GameObject> uiStack = new Stack<GameObject>();

    public bool IsUIActive => uiStack != null && uiStack.Count > 0;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Update()
    {
        // ESC 입력 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeInput();
        }
    }

    // ESC 키 로직 분리
    public void HandleEscapeInput()
    {
        // 스택에 쌓인 UI가 있다면 닫기
        if (uiStack.Count > 0)
        {
            GameObject topUI = uiStack.Peek();

            DungeonUIManager dungeonManager = FindObjectOfType<DungeonUIManager>();

            if (dungeonManager != null && topUI == dungeonManager.FullMap)
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
        GameSettingUI gameSettingUI = FindObjectOfType<GameSettingUI>();
        if (gameSettingUI != null)
        {
            gameSettingUI.OpenCloseSettingUI();
        }
    }

    // UI 열릴 때 호출
    public void RegisterUI(GameObject ui)
    {
        if (ui == null) return;

        // 이미 스택에 있는 경우 중복 등록 방지
        if (!uiStack.Contains(ui))
        {
            uiStack.Push(ui);
        }
    }

    // UI 닫힐 때 호출
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
                if (go != ui) uiStack.Push(go);
            }
        }
    }

    // 씬 전환 시 스택 초기화용
    public void ClearStack()
    {
        uiStack.Clear();
    }
}