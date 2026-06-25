using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Reroll : MonoBehaviour
{
    [SerializeField]
    private ShopManager itemSlotManager;
    [SerializeField]
    private TextMeshPro rerollCostTxt;

    private int rerollCoast;
    public bool inRoll = false;
    private bool canReroll;

    private PlayerStatus playerStatus;

    private void Awake()
    {
        rerollCoast = 50;
        UpdateRerollText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inRoll)
        {
            // 플레이어가 범위 내에 있을 때만 골드 체크 및 리롤 진행
            if (playerStatus != null)
            {
                if (rerollCoast <= playerStatus.Gold)
                    canReroll = true;
                else
                    canReroll = false;

                if (canReroll)
                {
                    RerollItem();
                }
                else
                {
                    Debug.Log("돈이 부족합니다.");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRoll = true;

            // PlayerManager 싱글톤을 이용하여 현재 플레이어의 PlayerStatus 컴포넌트를 가져옵니다.
            if (PlayerManager.Instance.CurrentPlayer != null)
            {
                playerStatus = PlayerManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
            }
            // 만약 PlayerManager에 할당이 안 되어있다면 트리거된 오브젝트에서 직접 가져오는 방어 코드
            else
            {
                playerStatus = collision.GetComponent<PlayerStatus>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRoll = false;
            playerStatus = null; // 범위를 벗어나면 참조를 해제하여 안전하게 관리합니다.
        }
    }

    private void RerollItem()
    {
        playerStatus.Gold -= rerollCoast;
        itemSlotManager.SellingItem();

        if (rerollCoast < 200)
        {
            rerollCoast += 50;
        }

        UpdateRerollText();
    }

    private void UpdateRerollText()
    {
        rerollCostTxt.text = $"Reroll\n<color=yellow>{rerollCoast}G</color>";
    }
}