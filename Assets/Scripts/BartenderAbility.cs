using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BartenderAbility : MonoBehaviour
{
    public int Bartender_Bottle = 10;

    public void Bartender_AttackDebuff()
    {

    }

    public void CheckBartenderAbility()
    {
        if (Bartender_Bottle > 0)
        {
            Bartender_Bottle--;
        }
    }
}
