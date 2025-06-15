using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlacksmithAbilityUI : AbilityUIBase
{
    [SerializeField] private Text weaponText;
    [SerializeField] private Button enchantButton;
    [SerializeField] private Button destroyButton;

    private BlacksmithAbilityV2 blacksmithAbility;

    public override void BindAbility(IAbilityV2 ability)
    {
        blacksmithAbility = ability as BlacksmithAbilityV2;

        if (blacksmithAbility != null)
        {
            blacksmithAbility.onAbilityUpdated.AddListener(UpdateUI);
            UpdateUI();
        }
    }

    public override void UpdateUI()
    {
        if (blacksmithAbility == null) return;


        if (blacksmithAbility.IsActivated)
        {
            weaponText.text = blacksmithAbility.CurWeaponData.name;
            enchantButton.GetComponentInChildren<Text>().text = "강화(+" + blacksmithAbility.EnchantLevel + ")";
            enchantButton.gameObject.SetActive(true);
            destroyButton.GetComponentInChildren<Text>().text = "폐기";
            destroyButton.gameObject.SetActive(true);
        }
        else
        {
            weaponText.text = "현재 제작 무기 없음";
            enchantButton.gameObject.SetActive(false);
            destroyButton.gameObject.SetActive(false);
        }
    }

    public void EnchantWeapon()
    {
        blacksmithAbility?.EnchantWeapon();
    }

    public void DestroyWeapon()
    {
        blacksmithAbility?.Deactivate();
    }
}
