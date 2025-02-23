using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Reroll : MonoBehaviour
{
    [SerializeField]
    ShopManager itemSlotManager;
    [SerializeField]
    TextMeshPro rerollCoastTxt;
    private int rerollCoast;
    private int rerollCount;

    public bool inRoll = false;
    private bool canReroll;
    [SerializeField]
    private PlayerStatus playerStatus;

    private void Awake()
    {
        rerollCount = 0;
        rerollCoast = 50;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (rerollCoast <= playerStatus.Gold)
                canReroll = true;
            else
                canReroll = false;
            
            if (canReroll && inRoll)
            {
                RerollItem();
            }
            else if (!canReroll && inRoll)
            {
                Debug.Log("돈이 부족합니다.");
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRoll = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRoll = false;
        }
    }
    private void RerollItem()
    {
        playerStatus.Gold -= rerollCoast;
        itemSlotManager.SellingItem();
        rerollCount++;
        if (rerollCoast != 200)
        {
            rerollCoastTxt.text = ((rerollCount + 1) * 50).ToString() + "G";
            rerollCoast += 50;
        }
        else
        {
            rerollCoastTxt.text = "200G";
            rerollCoast = 200;
        }
    }
}
