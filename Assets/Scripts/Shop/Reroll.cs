using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Reroll : MonoBehaviour
{
    [SerializeField]
    ItemSlotManager itemSlotManager;
    [SerializeField]
    TextMeshPro rerollCoastTxt;
    private int rerollCoast;
    private int rerollCount;

    private PlayerStatus playerStatus;

    private void Awake()
    {
        rerollCount = 0;
        rerollCoast = 50;
        playerStatus = GetComponent<PlayerStatus>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (collision.name == "player")
            {
                playerStatus.Gold -= rerollCoast;
                itemSlotManager.SellingItem();
                rerollCount++;
                if (rerollCoast != 200)
                {
                    rerollCoastTxt.text = ((rerollCount + 1) * 50).ToString() + "G";
                    rerollCoast +=50;
                }
                else
                {
                    rerollCoastTxt.text = ((rerollCount + 1) * 50).ToString() + "G";
                    rerollCoast = 200;
                }
            }
        }
    }
}
