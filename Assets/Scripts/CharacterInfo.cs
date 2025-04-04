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

    private void Awake()
    {
        Init();
        SetStatus();
        transform.GetComponentInChildren<InventoryUI>().Init();
        DontDestroyOnLoad(transform.parent);
        DisableUI();
    }

    private void OnEnable()
    {
        Init();
        SetStatus();
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
        if (playerStatus == null)
        {
            // GameManager에서 현재 플레이어 오브젝트를 가져와 PlayerStatus 컴포넌트 할당
            if (GameManager.Instance.CurrentPlayer != null)
            {
                playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
                if (playerStatus == null)
                {
                    Debug.Log("없음");
                }
            }
        }

    }

    // 스테이터스 세팅
    void SetStatus()
    {
        if (playerStatus == null)
        {
            if (GameManager.Instance.CurrentPlayer != null)
            {
                playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
                if (playerStatus == null)
                    Debug.Log("없음");
            }
        }

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
}
