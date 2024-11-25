using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Reroll : MonoBehaviour
{
    Collider2D rerollCollider;

    [SerializeField]
    ItemSlotManager itemSlotManager;
    [SerializeField]
    TextMeshPro rerollCoast;
    private int rerollCount;

    public int RerollCount {  get { return rerollCount; } set { rerollCount = value; } }  

    private void Awake()
    {
        rerollCollider = GetComponent<Collider2D>();
        rerollCount = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            itemSlotManager.SellingItem();
            rerollCount++;
            if (rerollCoast.text != "200G")
            {
                rerollCoast.text = ((rerollCount+1) * 50).ToString()+"G";
            }
            else
            {

            }
        }
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        if (collision.name == "Player")
    //        {
    //            //돈 차감하고 
    //            itemSlotManager.SellingItem();
    //            rerollCount++;
    //            if (rerollCoast.text != "200G")
    //            {
    //                rerollCoast.text = (rerollCount * 50).ToString();

    //            }
    //            else
    //            {
    //                //최대 리롤골드 200G?
    //            }
    //        }
    //    }
    //}
}
