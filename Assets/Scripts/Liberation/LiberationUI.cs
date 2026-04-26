using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static PlayerObj;

public class LiberationUI : MonoBehaviour
{
    [SerializeField] private GameObject liberationUI;
    [SerializeField] private LiberationSystem liberationSystem;
    [SerializeField] private LiberationOnEnable liberationOnEnable;

    [SerializeField] private Text playerNameText;
    [SerializeField] private Image playerImage;

    [System.Serializable]
    public struct PlayerSpriteData
    {
        public string playerName;
        public Sprite sprite;
    }
    [Header("플레이어 이미지 목록")]
    [SerializeField]
    private List<PlayerSpriteData> spriteList;

    private Dictionary<string, Sprite> playerSprites = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (liberationSystem != null)
        {
            liberationOnEnable.OnStatusChanged += HandleSystemStatus;
        }

        if (liberationOnEnable.gameObject.activeInHierarchy)
        {
            HandleSystemStatus(true);
        }

        foreach (var data in spriteList)
        {
            if (!playerSprites.ContainsKey(data.playerName))
                playerSprites.Add(data.playerName, data.sprite);
        }
        liberationUI.SetActive(false);
    }

    private void OnDestroy()
    {
        if (liberationSystem != null)
        {
            liberationOnEnable.OnStatusChanged -= HandleSystemStatus;
        }
    }

    private void HandleSystemStatus(bool isActive)
    {
        if (isActive)
        {
            liberationSystem.SyncSteadfiteText();
            liberationSystem.playerName = GameManager.Instance.CurrentPlayer.GetComponent<PlayerControllerBase>().playerName;
            liberationSystem.SyncLiberationData(liberationSystem.playerName);
            SetPlayerNameText(liberationSystem.playerName);
            SetPlayerImage(liberationSystem.playerName);
            SaveManager.Instance.SyncFromLiberationData();
        }
        else
        {
            Debug.Log("LiberationSystem 비활성화됨!");
        }
    }

    public void SetPlayerNameText(string playerName)
    {
        switch (playerName)
        {
            case "bartender":
                playerNameText.text = "바텐더";
                break;
            case "blacksmith":
                playerNameText.text = "대장장이";
                break;
            default:
                playerNameText.text = "";
                break;
        }
    }

    public void SetPlayerImage(string playerName)
    {
        playerImage.sprite = playerSprites[playerName];
    }

    public void CloseUIOnclick()
    {
        if (liberationUI != null) liberationUI.SetActive(false);
    }
}
