using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Reroll : MonoBehaviour
{
    [SerializeField]
    ItemSlotManager itemSlotManager;
    [SerializeField]
    TextMeshPro rerollCoastTxt;
    private int rerollCoast;
    private int rerollCount;

    public bool canReroll = false;
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
            if (canReroll)
            {
                RerollItem();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canReroll = true;
            Debug.Log(canReroll);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            canReroll = false;
            Debug.Log(canReroll);
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
