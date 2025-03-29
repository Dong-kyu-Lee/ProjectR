using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BartenderAbility : MonoBehaviour
{
    private PlayerBuffManager enemyBuffManager;
    public int Bartender_Bottle = 10;

    public void Bartender_AttackDebuff(GameObject enemy)
    {
        enemyBuffManager = enemy.GetComponent<PlayerBuffManager>();
        enemyBuffManager.ActivateBuff(BuffType.Buzzed, 10.0f);
    }

    public void CheckBartenderAbility()
    {
        if (Bartender_Bottle > 0)
        {
            Bartender_Bottle--;
        }
    }
}
