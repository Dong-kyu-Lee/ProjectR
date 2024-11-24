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

    public void EnableForceEffect4(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
        playerStatus.Damage += 10;
    }

    public void EnableForceEffect7(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
        playerStatus.Damage += 15;
    }

    public void EnableForceEffect10(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
        playerStatus.Damage += 20;
    }

    public void EnableForceEffect13(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
        playerStatus.Damage += 25;
    }

    public void EnableForceEffect16(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
        playerStatus.Damage += 30;
    }

    public void DisableForceEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
        playerStatus.Damage -= 5;
    }

    public void DisableForceEffect4(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
        playerStatus.Damage -= 10;
    }

    public void DisableForceEffect7(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
        playerStatus.Damage -= 15;
    }

    public void DisableForceEffect10(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
        playerStatus.Damage -= 20;
    }

    public void DisableForceEffect13(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
        playerStatus.Damage -= 25;
    }

    public void DisableForceEffect16(PlayerStatus playerStatus)
    {
        statusEffectTooltip = GameObject.Find("ForceEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
        playerStatus.Damage -= 30;
    }
}
