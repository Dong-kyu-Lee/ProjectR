using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlacksmithAbilityUI : AbilityUIBase
{
    [SerializeField] private Text weaponNameText;
    [SerializeField] private Text enchantLevelText;
    [SerializeField] private Text rankText;
    [SerializeField] private Image weaponImage;

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

        var weaponData = blacksmithAbility.CurWeaponData;

        weaponNameText.text = weaponData.WeaponName;
        weaponImage.sprite = weaponData.WeaponSprite;

        if (blacksmithAbility.IsActivated)
        {
            enchantLevelText.text = $"+{blacksmithAbility.EnchantLevel}";
            rankText.text = GetGradeName(weaponData.Rank);

            enchantButton.gameObject.SetActive(true);
            destroyButton.gameObject.SetActive(true);
        }
        else
        {
            enchantLevelText.text = "-";
            rankText.text = "-";

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

    private string GetGradeName(int rank)
    {
        return rank switch
        {
            1 => "C",
            2 => "B",
            3 => "A",
            4 => "S",
            _ => "-"
        };
    }
}
