using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public GameObject characterName;
    public GameObject statusTextPref;
    public GameObject statusParentObj;
    public PlayerStatus playerStatus;

    List<GameObject> statusObjList = new List<GameObject>();

    //장비칸 UI 관련 변수들
    [SerializeField]
    private GameObject equipSlotParentObj;
    [SerializeField]
    private GameObject itemSlotPref;
    private List<RawImage> equipSlotImgs = null;

    //인벤토리 UI 관련 변수
    [SerializeField]
    private GameObject[] inventorySlotParentObj;

    //플레이어 인벤토리
    [SerializeField]
    private Inventory playerInventory;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //장비 해제
            UpdateAllInventorySlotImages();
            UpdateAllEquippedItemSlotImages();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //장비 칸끼리 교체
            //UpdateAllEquippedItemSlotImages();
            UpdateEquippedItemSlotImage(0);
            UpdateEquippedItemSlotImage(4);
        }
    }

    private void OnEnable()
    {
        Init();
        SetStatus();
        UpdateAllEquippedItemSlotImages();
        UpdateAllInventorySlotImages();
    }

    // UI 활성화
    public void EnableUI()
    {
        gameObject.SetActive(true);
    }

    // UI 비활성화
    public void DisableUI()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    // 세팅 전 초기화
    void Init()
    {
        Debug.Log("Init");
        statusObjList.Clear();
        if (equipSlotImgs == null) GenerateEquippedItemSlot();
    }

    // 스테이터스 세팅
    void SetStatus()
    {
        characterName.GetComponent<Text>().text = "바텐더";

        for (int i = 0; i <= 8; i++)
        {
            GameObject statusObj = Instantiate(statusTextPref);
            statusObj.transform.SetParent(statusParentObj.transform);
            statusObjList.Add(statusObj);
        }

        float additionalDamageValue = playerStatus.Damage * playerStatus.AdditionalDamage * 0.01f;
        float additionalDamageReductionValue = playerStatus.DamageReduction * playerStatus.AdditionalDamageReduction * 0.01f;
        statusObjList[0].GetComponent<Text>().text = "레벨 : " + playerStatus.Level;
        // 경험치 추가 예정.
        statusObjList[1].GetComponent<Text>().text = "피해량 : " + playerStatus.TotalDamage + "(" + playerStatus.Damage + "+" + "<color=yellow>" + additionalDamageValue + "</color>" + "<color=black>)</color>";
        statusObjList[2].GetComponent<Text>().text = "추가 피해량 : " + playerStatus.AdditionalDamage + "%";
        statusObjList[3].GetComponent<Text>().text = "치명타 확률 : " + playerStatus.CriticalPercent + "%";
        statusObjList[4].GetComponent<Text>().text = "피해 감소량 : " + playerStatus.TotalDamageReduction + "(" + playerStatus.DamageReduction + "+" + "<color=yellow>" + additionalDamageReductionValue + "</color>" + "<color=black>)</color>";
        statusObjList[5].GetComponent<Text>().text = "추가 피해 감소량 : " + playerStatus.AdditionalDamageReduction + "%";
        statusObjList[6].GetComponent<Text>().text = "공격속도 : " + playerStatus.AttackSpeed;
        statusObjList[7].GetComponent<Text>().text = "이동속도 : " + playerStatus.MoveSpeed;
        statusObjList[8].GetComponent<Text>().text = "재화 획득량 : " + playerStatus.PriceAdditional;
    }

    //장비칸 UI를 생성하는 함수(LazyInstantiation)
    private void GenerateEquippedItemSlot()
    {
        equipSlotImgs = new List<RawImage>();
        for (int i = 0; i < playerInventory.EquipmentItemSlot.Length; i++)
        {
            GameObject itemSlot = Instantiate(itemSlotPref, equipSlotParentObj.transform);
            itemSlot.GetComponent<RectTransform>().localPosition
                = new Vector3(-90 + (i % 3) * 90, -110 * (i / 3) + 20, 0);  //3은 가로행 수(row)

            equipSlotImgs.Add(itemSlot.transform.GetChild(0).GetComponent<RawImage>());
        }
    }

    //모든 장비칸 이미지 업데이트 함수
    private void UpdateAllEquippedItemSlotImages()
    {
        for (int i = 0; i < equipSlotImgs.Count; i++)
        {
            UpdateEquippedItemSlotImage(i);
        }
    }

    private void UpdateEquippedItemSlotImage(int slotIdx)
    {
        equipSlotImgs[slotIdx].GetComponent<RawImage>().texture
            = playerInventory.EquipmentItemSlot[slotIdx].ItemSprite.texture;
    }

    //해당 장비칸 이미지 업데이트 함수
    public void ChangeEquippedItemSlotImage(Texture texture, int slotIndex)
    {
        equipSlotImgs[slotIndex].GetComponent<RawImage>().texture = texture;
    }

    //모든 인벤토리 슬롯 이미지 업데이트 함수
    private void UpdateAllInventorySlotImages()
    {
        int row = 0;
        int col = 0;

        foreach(var item in playerInventory.InventoryDict)
        {
            ChangeInventorySlotImage(item.Key.ItemSprite, row, col);
            if (col > inventorySlotParentObj[row].transform.childCount)
            {
                col = 0;
                row++;
            }
            else
            {
                col++;
            }
        }
    }

    //해당 인벤토리 슬롯 이미지 업데이트 함수
    public void ChangeInventorySlotImage(Sprite targetSprite, int slotRowIdx, int slotColIdx)
    {
        inventorySlotParentObj[slotRowIdx].transform.GetChild(slotColIdx).GetComponent<Image>().sprite = targetSprite;
    }
}
