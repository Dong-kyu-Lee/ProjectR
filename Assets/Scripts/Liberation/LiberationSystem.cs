using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerObj;

public class LiberationSystem : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private Text currentSteadfiteText;

    public string characterName;
    public int currentAbility = -1;
    [SerializeField] private LiberationDesc[] liberationDesc = new LiberationDesc[6];

    private void Awake()
    {
        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        characterName = "bartender";
        currentSteadfiteText.text = playerStatus.Steadfite.ToString();
    }

    private void Start()
    {
        SyncLiberationColor();
    }

    public void EnableLiberationOnClick()
    {
        if (AbilityManager.Instance.bartenderAbility[currentAbility])
        {
            Debug.Log("이미 활성화된 능력입니다.");
        }
        else if (playerStatus.Steadfite < liberationDesc[currentAbility].abilityPrice)
        {
            Debug.Log("능력을 해방하기 위한 단석이 부족합니다.");
        }
        else
        {
            playerStatus.Steadfite -= liberationDesc[currentAbility].abilityPrice;
            currentSteadfiteText.text = playerStatus.Steadfite.ToString();
            EnableLiberationEffect(characterName, currentAbility);
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

    // 바텐더의 해방 효과 연동.
    public void SyncLiberationColor()
    {
        for (int point = 0; point <= 5; point++)
        {
            if (AbilityManager.Instance.bartenderAbility[point]) liberationDesc[point].defaultColor = Color.white;
            else liberationDesc[point].defaultColor = new Color(100f / 255f, 100f / 255f, 100f / 255f);
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
