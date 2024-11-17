using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceEffect : MonoBehaviour
{
    private StatusEffectTooltip statusEffectTooltip;

    public void EnableForceEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
        playerStatus.Damage += 5;
    }

    public void EnableForceEffect4()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableForceEffect7()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableForceEffect10()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableForceEffect13()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableForceEffect16()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void DisableForceEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
        playerStatus.Damage -= 5;
    }

    public void DisableForceEffect4()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableForceEffect7()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableForceEffect10()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableForceEffect13()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableForceEffect16()
    {
        statusEffectTooltip = GameObject.Find("ForceEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }
}
