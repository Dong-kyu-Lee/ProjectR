using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDescriptionPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text itemName;   //아이템 이름
    [SerializeField]
    private TMP_Text itemDescription;    //아이템 설명

    public string ItemName { get => itemName.text; set => itemName.text = value; }
    public string ItemDescription { get => itemDescription.text; set => itemDescription.text = value; }
}
