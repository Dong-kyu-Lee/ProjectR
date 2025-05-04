using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blacksmith Weapon Data", menuName = "Scriptable Object/Blacksmith Weapon Data", order = 1)]

public class BlacksmithWeaponData : ScriptableObject
{
    [SerializeField]
    string weaponName;

    [SerializeField]
    int weaponType;

    [SerializeField]
    int rank;

    [SerializeField]
    int enchantLevel;

    [SerializeField]
    Sprite weaponSprite;

    [SerializeField]
    float addtionalDamage;

    [SerializeField]
    float addtionalAttackSpeed;

    public string WeaponName { get { return weaponName; } }
    public int WeaponType { get { return weaponType; } }
    public int Rank { get { return rank; } set { rank = value; } }
    public int EnchantLevel { get { return enchantLevel; } set { enchantLevel = value; } }
}
