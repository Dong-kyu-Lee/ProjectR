using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static LiberationUI;
using static PlayerObj;

[System.Serializable]
public class LiberationAbilityDesc
{
    public string abilityDesc;
    public int abilityPrice;
}

public class LiberationSystem : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private Text currentSteadfiteText;

    public string playerName;
    public int currentAbility = -1;
    [SerializeField] private LiberationDesc[] liberationDesc = new LiberationDesc[6];

    [Header("바텐더 능력")]
    public List<LiberationAbilityDesc> bartenderAbilities = new List<LiberationAbilityDesc>(6);

    [Header("대장장이 능력")]
    public List<LiberationAbilityDesc> blacksmithAbilities = new List<LiberationAbilityDesc>(6);

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (GameManager.Instance.CurrentPlayer == null)
        {
            Debug.LogError("CurrentPlayer가 아직 비어있습니다!");
        }
        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        playerName = GameManager.Instance.CurrentPlayer.GetComponent<PlayerControllerBase>().playerName;
        SyncLiberationData(playerName);
    }

    public void SyncSteadfiteText()
    {
        currentSteadfiteText.text = SaveManager.Instance.GetSteadfite().ToString();
    }

    public void GetPlayerStatus(PlayerStatus playerStat)
    {
        playerStatus = playerStat;
    }

    public void EnableLiberationOnClick()
    {
        if (AbilityManager.Instance.bartenderAbility[currentAbility])
        {
            Debug.Log("이미 활성화된 능력입니다.");
        }
        else if (SaveManager.Instance.GetSteadfite() < liberationDesc[currentAbility].AbilityPrice)
        {
            Debug.Log("능력을 해방하기 위한 단석이 부족합니다.");
        }
        else
        {
            SaveManager.Instance.AddSteadfite(-liberationDesc[currentAbility].AbilityPrice);
            currentSteadfiteText.text = SaveManager.Instance.GetSteadfite().ToString();
            EnableLiberationEffect(playerName, currentAbility);
        }
    }

    // 해당 캐릭터의 해방 효과 리셋.
    public void ResetLiberation(string characterName)
    {
        for (int point = 0; point <= 5; point++)
        {
            DisableLiberationEffect(characterName, point);
        }
    }

    // 현재 플레이어의 해방 효과 연동.
    public void SyncLiberationData(string characterName)
    {
        for (int point = 0; point <= 5; point++)
        {
            switch (characterName)
            {
                case "bartender":
                    if (AbilityManager.Instance.bartenderAbility[point]) liberationDesc[point].defaultColor = Color.white;
                    else liberationDesc[point].defaultColor = new Color(100f / 255f, 100f / 255f, 100f / 255f); // 회색
                    liberationDesc[point].SetAbilityDesc("");
                    break;
                case "blacksmith":
                    if (AbilityManager.Instance.blacksmithAbility[point]) liberationDesc[point].defaultColor = Color.white;
                    else liberationDesc[point].defaultColor = new Color(100f / 255f, 100f / 255f, 100f / 255f); // 회색
                    break;
            }
            liberationDesc[point].image.color = liberationDesc[point].defaultColor;
        }
    }

    // 해방 특수 효과 활성화.
    public void EnableLiberationEffect(string characterName, int point)
    {
        liberationDesc[point].defaultColor = Color.white;
        liberationDesc[point].image.color = liberationDesc[point].defaultColor;
        AbilityManager.Instance.SetAbiltiy(characterName, point, true);
    }

    // 해방 특수 효과 비활성화.
    public void DisableLiberationEffect(string characterName, int point)
    {
        liberationDesc[point].defaultColor = new Color(100f / 255f, 100f / 255f, 100f / 255f);
        liberationDesc[point].image.color = liberationDesc[point].defaultColor;
        AbilityManager.Instance.SetAbiltiy(characterName, point, false);
    }
}
