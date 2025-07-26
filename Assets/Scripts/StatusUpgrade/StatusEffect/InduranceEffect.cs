using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerObj;

public class InduranceEffect : MonoBehaviour
{
    private StatusEffectTooltip[] statusEffectTooltip = new StatusEffectTooltip[6];

    private void Awake()
    {
        statusEffectTooltip[0] = transform.Find("InduranceEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[1] = transform.Find("InduranceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[2] = transform.Find("InduranceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[3] = transform.Find("InduranceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[4] = transform.Find("InduranceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[5] = transform.Find("InduranceEffect16").GetComponent<StatusEffectTooltip>();
    }

    public void EnableInduranceEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.yellow;
        statusEffectTooltip[0].image.color = Color.yellow;
        playerStatus.MaxHp += 20;
        playerStatus.Hp += 20;
    }

    public void EnableInduranceEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.yellow;
        statusEffectTooltip[1].image.color = Color.yellow;
        CalcReceiveDamage.Instance.induranceEffect4 = true;
    }

    public void EnableInduranceEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.yellow;
        statusEffectTooltip[2].image.color = Color.yellow;
        CalcReceiveDamage.Instance.induranceEffect7 = true;
    }

    public void EnableInduranceEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.yellow;
        statusEffectTooltip[3].image.color = Color.yellow;
        CalcReceiveDamage.Instance.induranceEffect10 = true;
        CalcReceiveDamage.Instance.InduranceEffect10_IncreaseDamage();
    }

    public void EnableInduranceEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.yellow;
        statusEffectTooltip[4].image.color = Color.yellow;
        CalcReceiveDamage.Instance.induranceEffect13 = true;
        CalcReceiveDamage.Instance.InduranceEffect13_IncreaseDamageReduction();
    }

    public void EnableInduranceEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.yellow;
        statusEffectTooltip[5].image.color = Color.yellow;
        CalcReceiveDamage.Instance.induranceEffect16 = true;
    }

    public void DisableInduranceEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
        playerStatus.MaxHp -= 20;
    }

    public void DisableInduranceEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
        CalcReceiveDamage.Instance.induranceEffect4 = false;
    }

    public void DisableInduranceEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
        CalcReceiveDamage.Instance.induranceEffect7 = false;
    }

    public void DisableInduranceEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
        CalcReceiveDamage.Instance.induranceEffect10 = false;
        CalcReceiveDamage.Instance.InduranceEffect10_IncreaseDamage();
    }

    public void DisableInduranceEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
        CalcReceiveDamage.Instance.induranceEffect13 = false;
        CalcReceiveDamage.Instance.InduranceEffect13_IncreaseDamageReduction();
    }

    public void DisableInduranceEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
        CalcReceiveDamage.Instance.induranceEffect16 = false;
    }
}
