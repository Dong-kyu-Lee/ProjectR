using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using static PlayerObj;

public class LiberationSystem : MonoBehaviour
{
    private HashSet<string> unlockedEffects = new HashSet<string>(); // 중복 실행 방지용.

    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private Text currentSteadfiteText;

    public string characterName;
    public int currentAbility = -1;
    private LiberationDesc[] liberationDesc = new LiberationDesc[6];

    private void Awake()
    {
        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        liberationDesc = GetComponentsInChildren<LiberationDesc>();
        characterName = "bartender";
        currentSteadfiteText.text = playerStatus.Steadfite.ToString();
    }

    private void Start()
    {
        playerStatus.Steadfite = 3000;
    }

    public void EnableLiberationOnClick()
    {
        if (unlockedEffects.Contains($"{characterName}_{currentAbility}"))
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
            if (!unlockedEffects.Contains($"{characterName}_{currentAbility}"))
            {
                EnableLiberationEffect(characterName, currentAbility);
                unlockedEffects.Add($"{characterName}_{currentAbility}");
            }
        }
    }

    // 해당 캐릭터의 해방 효과 리셋.
    private void ResetLiberation(string characterName)
    {
        for (int point = 0; point <= 5; point++)
        {
            if (unlockedEffects.Contains($"{characterName}_{point}"))
            {
                DisableLiberationEffect(characterName, point);
                unlockedEffects.Remove($"{characterName}_{point}");
            }
        }
    }

    // 해방 특수 효과 활성화.
    public void EnableLiberationEffect(string characterName, int point)
    {
        AbilityManager.Instance.SetAbiltiy(characterName, point, true);
    }

    // 해방 특수 효과 비활성화.
    public void DisableLiberationEffect(string characterName, int point)
    {
        AbilityManager.Instance.SetAbiltiy(characterName, point, false);
    }
}
