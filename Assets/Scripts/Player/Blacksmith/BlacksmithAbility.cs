using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithAbility : MonoBehaviour, IAbility
{
    bool isActivated;

    [SerializeField]
    BlacksmithWeaponData curWeaponData;

    [SerializeField]
    BlacksmithWeaponData[] weaponDataList;

    public void Activate()
    {
        if (!isActivated)
        {
            isActivated = true;
            curWeaponData = weaponDataList[1];
            Debug.Log("CurWeapon : " + curWeaponData.WeaponName);
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
