using System;
using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    private static InteractionUI instance;
    public static InteractionUI Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<InteractionUI>();
            return instance;
        }
    }

    [SerializeField] private TextMeshProUGUI warpUIText;
    private Action warpAction;

    private void Awake()
    {
        if (instance == null) instance = this;

        // 시작할 때 워프 텍스트가 켜져 있다면 꺼줌
        if (warpUIText != null) warpUIText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 워프 텍스트가 켜져 있을 때 E 키를 누르면 저장된 액션 실행
        if (Input.GetKeyDown(KeyCode.E) && warpUIText != null && warpUIText.gameObject.activeSelf)
        {
            warpAction?.Invoke();
        }
    }

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
        if (warpUIText != null)
        {
            warpUIText.gameObject.SetActive(false);
            warpAction = null;
        }
    }
}