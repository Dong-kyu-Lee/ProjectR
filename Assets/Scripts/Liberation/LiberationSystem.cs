using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using static PlayerObj;

public class LiberationSystem : MonoBehaviour
{
    private HashSet<string> unlockedEffects = new HashSet<string>(); // 중복 실행 방지용.

    private float crystal;
    [SerializeField] private Text currentCrystalText;

    public string characterName;
    public int currentAbility = 0;
    private LiberationDesc[] liberationDesc = new LiberationDesc[6];

    private void Awake()
    {
        liberationDesc = GetComponentsInChildren<LiberationDesc>();
        characterName = "bartender";
        Crystal = 1000;
        currentCrystalText.text = crystal.ToString();
    }

    public float Crystal
    {
        get { return crystal; }
        set
        {
            crystal = value;
        }
    }

    public void EnableLiberationOnClick()
    {
        if (unlockedEffects.Contains($"{characterName}_{currentAbility}"))
        {
            Debug.Log("이미 활성화된 능력입니다.");
        }
        else if (Crystal < liberationDesc[currentAbility - 1].abilityPrice)
        {
            Debug.Log("능력을 해방하기 위한 크리스탈이 부족합니다.");
        }
        else
        {
            Crystal -= liberationDesc[currentAbility - 1].abilityPrice;
            currentCrystalText.text = crystal.ToString();
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
        for (int point = 1; point <= 6; point++)
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
        switch (characterName)
        {
            case "bartender":
                switch (point)
                {
                    case 1: break;
                    case 2: break;
                    case 3: break;
                    case 4: break;
                    case 5: break;
                    case 6: break;
                    default:
                        Debug.Log("올바르지 않는 숫자");
                        return;
                }
                break;
            default:
                Debug.Log("잘못된 직업 이름");
                return;
        }
    }

    // 해방 특수 효과 비활성화.
    public void DisableLiberationEffect(string characterName, int point)
    {
        switch (characterName)
        {
            case "bartender":
                switch (point)
                {
                    case 1: break;
                    case 2: break;
                    case 3: break;
                    case 4: break;
                    case 5: break;
                    case 6: break;
                    default:
                        Debug.Log("올바르지 않는 숫자");
                        return;
                }
                break;
            default:
                Debug.Log("잘못된 직업 이름");
                return;
        }
    }
}
