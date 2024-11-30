using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceEffect : MonoBehaviour
{
    private StatusEffectTooltip[] statusEffectTooltip = new StatusEffectTooltip[6];

    private void Awake()
    {
        statusEffectTooltip[0] = GameObject.Find("ForceEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[1] = GameObject.Find("ForceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[2] = GameObject.Find("ForceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[3] = GameObject.Find("ForceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[4] = GameObject.Find("ForceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[5] = GameObject.Find("ForceEffect16").GetComponent<StatusEffectTooltip>();
    }

    public void EnableForceEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.yellow;
        statusEffectTooltip[0].image.color = Color.yellow;
        playerStatus.Damage += 5;
    }

    public void EnableForceEffect4(PlayerStatus playerStatus)
    {
        statusEffectTooltip[1].defaultColor = Color.yellow;
        statusEffectTooltip[1].image.color = Color.yellow;
        playerStatus.Damage += 10;
    }

    public void EnableForceEffect7(PlayerStatus playerStatus)
    {
        statusEffectTooltip[2].defaultColor = Color.yellow;
        statusEffectTooltip[2].image.color = Color.yellow;
        playerStatus.Damage += 15;
    }

    public void EnableForceEffect10(PlayerStatus playerStatus)
    {
        statusEffectTooltip[3].defaultColor = Color.yellow;
        statusEffectTooltip[3].image.color = Color.yellow;
        playerStatus.Damage += 20;
    }

    public void EnableForceEffect13(PlayerStatus playerStatus)
    {
        statusEffectTooltip[4].defaultColor = Color.yellow;
        statusEffectTooltip[4].image.color = Color.yellow;
        playerStatus.Damage += 25;
    }

    public void EnableForceEffect16(PlayerStatus playerStatus)
    {
        statusEffectTooltip[5].defaultColor = Color.yellow;
        statusEffectTooltip[5].image.color = Color.yellow;
        playerStatus.Damage += 30;
    }

    public void DisableForceEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
        playerStatus.Damage -= 5;
    }

    public void DisableForceEffect4(PlayerStatus playerStatus)
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
        playerStatus.Damage -= 10;
    }

    public void DisableForceEffect7(PlayerStatus playerStatus)
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
        playerStatus.Damage -= 15;
    }

    public void DisableForceEffect10(PlayerStatus playerStatus)
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
        playerStatus.Damage -= 20;
    }

    public void DisableForceEffect13(PlayerStatus playerStatus)
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
        playerStatus.Damage -= 25;
    }

    public void DisableForceEffect16(PlayerStatus playerStatus)
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
        playerStatus.Damage -= 30;
    }
}
