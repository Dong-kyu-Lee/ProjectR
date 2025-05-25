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

    [SerializeField] private float additionalDamage;
    [SerializeField] private float additionalAttackSpeed;

    [SerializeField] private WeaponStyle weaponStyle;
    [SerializeField] private RuntimeAnimatorController animatorOverride;

    public string WeaponName => weaponName;
    public int WeaponType => weaponType;
    public int Rank { get => rank; set => rank = value; }
    public int EnchantLevel { get => enchantLevel; set => enchantLevel = value; }

    public float AdditionalDamage => additionalDamage;
    public float AdditionalAttackSpeed => additionalAttackSpeed;

    public Sprite WeaponSprite => weaponSprite;

    public WeaponStyle WeaponStyle => weaponStyle;
    public RuntimeAnimatorController AnimatorOverride => animatorOverride;

    public float GetEffectiveDamage()
    {
        return additionalDamage + enchantLevel * 0.1f;
    }

    public float GetEffectiveAttackSpeed()
    {
        return additionalAttackSpeed + enchantLevel * 0.02f;
    }
}
