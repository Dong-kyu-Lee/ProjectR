using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public bool bartenderAbility1;
    public bool bartenderAbility2;
    public bool bartenderAbility3;
    public bool bartenderAbility4;
    public bool bartenderAbility5;
    public bool bartenderAbility6;

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
}
