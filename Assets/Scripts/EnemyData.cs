using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemy Data", order = int.MaxValue)]
public class EnemyData : ScriptableObject
{
    [SerializeField]
    private string enemyName;
    [SerializeField]
    private float hp;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float damageReduction;
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float sightRange;

    public string Name { get { return name; } }
    public float Hp { get { return hp; } }
    public float Damage { get { return damage; } }
    public float DamageReduction { get { return damageReduction; } }
    public float AttackSpeed { get { return attackSpeed; } }
    public float MoveSpeed { get { return moveSpeed; } }
    public float AttackRange { get { return AttackRange; } }
    public float SightRange { get { return sightRange; } }
}
