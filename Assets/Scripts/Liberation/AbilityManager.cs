using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerObj;

public class AbilityManager : MonoBehaviour
{
    public bool[] bartenderAbility = new bool[6];
    public bool[] blacksmithAbility = new bool[6];

    private BlacksmithAbilityV2 blacksmith;

    private const float requireSoulShard = 200f;

    private static AbilityManager instance;

    public static AbilityManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject abilityManagerObject = new GameObject("AbilityManager");
                instance = abilityManagerObject.AddComponent<AbilityManager>();
                DontDestroyOnLoad(abilityManagerObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetAbility(string characterName, int point, bool enable)
    {
        switch (characterName)
        {
            case "bartender":
                bartenderAbility[point] = enable;
                break;
            case "blacksmith":
                blacksmithAbility[point] = enable;
                break;
            default:
                break;
        }
        SaveManager.Instance.SaveAbility(characterName, point, enable);
    }

    public void IncreaseSoulShard(float value)
    {
        GameObject player = GameManager.Instance.CurrentPlayer;
        string playerName = player.GetComponent<PlayerControllerBase>().playerName;

        if (playerName == "blacksmith") blacksmith = player.GetComponent<BlacksmithAbilityV2>();
        else return;

        if (blacksmithAbility[2]) value *= 1.5f;
        blacksmith.SoulShard += value;
        blacksmith.onAbilityUpdated.Invoke();

        if (blacksmith.SoulShard >= requireSoulShard)
        {
            IncreaseUpgradeChance();
        }
    }

    private void IncreaseUpgradeChance()
    {
        blacksmith.UpgradeChance++;
        blacksmith.SoulShard -= requireSoulShard;
        blacksmith.onAbilityUpdated.Invoke();

        if (blacksmith.SoulShard >= requireSoulShard)
        {
            IncreaseUpgradeChance();
        }
    }
}
