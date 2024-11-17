using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalEffect : MonoBehaviour
{
    private StatusEffectTooltip statusEffectTooltip;

    public void EnableCriticalEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
        playerStatus.CriticalDamage += 10;
    }

    public void EnableCriticalEffect4()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableCriticalEffect7()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableCriticalEffect10()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableCriticalEffect13()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableCriticalEffect16()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void DisableCriticalEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
        playerStatus.CriticalDamage -= 10;
    }

    public void DisableCriticalEffect4()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableCriticalEffect7()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableCriticalEffect10()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableCriticalEffect13()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableCriticalEffect16()
    {
        statusEffectTooltip = GameObject.Find("CriticalEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }
}
