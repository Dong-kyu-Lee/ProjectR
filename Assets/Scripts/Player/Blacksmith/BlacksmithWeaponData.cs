using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponStyle
{
    OneHanded,
    TwoHanded
}

[CreateAssetMenu(fileName = "Blacksmith Weapon Data", menuName = "Scriptable Object/Blacksmith Weapon Data", order = 1)]
public class BlacksmithWeaponData : ScriptableObject
{
    [SerializeField] private string weaponName;
    [SerializeField] private int weaponType;
    [SerializeField] private int rank;
    [SerializeField] private int enchantLevel;
    [SerializeField] private Sprite weaponSprite;

    [SerializeField] private float weaponDamage;
    [SerializeField] private float additionalDamage;
    [SerializeField] private float additionalAttackSpeed;

    [SerializeField] private WeaponStyle weaponStyle;
    [SerializeField] private RuntimeAnimatorController animatorOverride;

    [SerializeField] private float hitStunDuration = 0.3f;

    public string WeaponName => weaponName;
    public int WeaponType => weaponType;
    public int Rank { get => rank; set => rank = value; }
    public int EnchantLevel { get => enchantLevel; set => enchantLevel = value; }

    public float WeaponDamage => weaponDamage;
    public float AdditionalDamage => additionalDamage;
    public float AdditionalAttackSpeed => additionalAttackSpeed;

    public Sprite WeaponSprite => weaponSprite;

    public WeaponStyle WeaponStyle => weaponStyle;
    public RuntimeAnimatorController AnimatorOverride => animatorOverride;

    public float HitStunDuration => hitStunDuration;

    public float GetEffectiveDamage()
    {
        return weaponDamage + (rank - 1) * 5f + enchantLevel * 1f;
    }

    public float GetEffectiveAdditionalDamage()
    {
        if (weaponStyle == WeaponStyle.TwoHanded) return additionalDamage + (rank - 1) * 0.05f;
        else return additionalDamage;
    }

    public float GetEffectiveAttackSpeed()
    {
        if (weaponStyle == WeaponStyle.OneHanded) return additionalAttackSpeed + (rank - 1) * 0.1f;
        else return additionalAttackSpeed;
    }
}
