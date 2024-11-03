using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : Status
{
    void Awake()
    {
        Hp = 100;
        Damage = 10;
        DamageReduction = 0;
        AttackSpeed = 1;
        MoveSpeed = 2;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
