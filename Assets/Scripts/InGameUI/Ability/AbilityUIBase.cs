using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityUIBase : MonoBehaviour
{
    public abstract void BindAbility(IAbilityV2 ability);
    public abstract void UpdateUI();
}
