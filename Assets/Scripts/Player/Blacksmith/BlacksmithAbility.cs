using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlacksmithAbility : MonoBehaviour, IAbility
{
    bool isActivated;

    [SerializeField]
    int enchantLevel;

    [SerializeField]
    BlacksmithWeaponData curWeaponData;

    [SerializeField]
    BlacksmithWeaponData[] weaponDataList;

    public bool IsActivated { get { return isActivated; } }
    public int EnchantLevel { get { return enchantLevel; } }

    public UnityEvent onAbilityUpdated;

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            curWeaponData = weaponDataList[1];
            Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
            onAbilityUpdated.Invoke();   
        }
    }

    public void Deactivate()
    {
        Initialize();
        curWeaponData = weaponDataList[0];
        Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
        onAbilityUpdated.Invoke();
    }

    public void EnchantWeapon()
    {
        float prob = Random.Range(0f, 1f);
        if (prob > 0.5f)
        {
            Debug.Log(prob);
            Debug.Log("성공");
            ++enchantLevel;
        }
        onAbilityUpdated.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Initialize()
    {
        isActivated = false;
        enchantLevel = 0;
    }
}
