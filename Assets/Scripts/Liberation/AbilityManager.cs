using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public bool[] bartenderAbility = new bool[6];

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

    public void SetAbiltiy(string characterName, int point, bool enable)
    {
        bartenderAbility[point] = enable;
        SaveManager.Instance.SaveAbility(characterName, point, enable);
    }

    public void SyncAbility(int point, bool enable)
    {
        bartenderAbility[point] = enable;
    }
}
