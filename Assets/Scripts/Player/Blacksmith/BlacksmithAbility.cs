using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlacksmithAbility : MonoBehaviour, IAbility
{
    bool isActivated;

    [SerializeField]
    BlacksmithWeaponData curWeaponData;

    [SerializeField]
    BlacksmithWeaponData[] weaponDataList;

    public bool IsActivated { get { return isActivated; } }

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
        else
        {
            Deactivate();
        }
    }

    public void Deactivate()
    {
        isActivated = false;
        curWeaponData = weaponDataList[0];
        Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
        onAbilityUpdated.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        isActivated = false;

        Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
