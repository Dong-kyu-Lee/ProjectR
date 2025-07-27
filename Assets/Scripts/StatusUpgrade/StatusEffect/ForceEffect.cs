using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceEffect : MonoBehaviour
{
    private StatusEffectTooltip[] statusEffectTooltip = new StatusEffectTooltip[6];

    private void Awake()
    {
        statusEffectTooltip[0] = transform.Find("ForceEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[1] = transform.Find("ForceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[2] = transform.Find("ForceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[3] = transform.Find("ForceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[4] = transform.Find("ForceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[5] = transform.Find("ForceEffect16").GetComponent<StatusEffectTooltip>();
    }

    public void EnableForceEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.yellow;
        statusEffectTooltip[0].image.color = Color.yellow;
        playerStatus.Damage += 5;
    }

    public void EnableForceEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.yellow;
        statusEffectTooltip[1].image.color = Color.yellow;
        CalcDamage.Instance.forceEffect4 = true;
    }

    public void EnableForceEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.yellow;
        statusEffectTooltip[2].image.color = Color.yellow;
        CalcDamage.Instance.forceEffect7 = true;
    }

    public void EnableForceEffect10(PlayerStatus playerStatus)
    {
        statusEffectTooltip[3].defaultColor = Color.yellow;
        statusEffectTooltip[3].image.color = Color.yellow;
        playerStatus.IgnoreDamageReduction = 1 - (1 - playerStatus.IgnoreDamageReduction) * (1 - 0.3f);
    }

    public void EnableForceEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.yellow;
        statusEffectTooltip[4].image.color = Color.yellow;
        CalcDamage.Instance.forceEffect13 = true;
    }

    public void EnableForceEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.yellow;
        statusEffectTooltip[5].image.color = Color.yellow;
        CalcDamage.Instance.forceEffect16 = true;
    }

    public void DisableForceEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
        playerStatus.Damage -= 5;
    }

    public void DisableForceEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
        CalcDamage.Instance.forceEffect4 = false;
    }

    public void DisableForceEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
        CalcDamage.Instance.forceEffect7 = false;
        CalcDamage.Instance.ResetBuff();
    }

    public void DisableForceEffect10(PlayerStatus playerStatus)
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
        playerStatus.IgnoreDamageReduction = 1 - (1 - playerStatus.IgnoreDamageReduction) / (1 - 0.3f);
    }

    public void DisableForceEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
        CalcDamage.Instance.forceEffect13 = false;
    }

    public void DisableForceEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
        CalcDamage.Instance.forceEffect16 = false;
        CalcDamage.Instance.ResetBuff();
    }
}
