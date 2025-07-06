using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerObj;

public class CurseEffect : MonoBehaviour
{
    private StatusEffectTooltip[] statusEffectTooltip = new StatusEffectTooltip[6];

    private void Awake()
    {
        statusEffectTooltip[0] = transform.Find("CurseEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[1] = transform.Find("CurseEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[2] = transform.Find("CurseEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[3] = transform.Find("CurseEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[4] = transform.Find("CurseEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[5] = transform.Find("CurseEffect16").GetComponent<StatusEffectTooltip>();
    }

    public void EnableCurseEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.yellow;
        statusEffectTooltip[0].image.color = Color.yellow;
        playerStatus.DebuffDamage += 0.15f;
    }

    public void EnableCurseEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.yellow;
        statusEffectTooltip[1].image.color = Color.yellow;
        CalcDamage.Instance.curseEffect4 = true;
    }

    public void EnableCurseEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.yellow;
        statusEffectTooltip[2].image.color = Color.yellow;
        CalcDamage.Instance.additionalDebuffTime = 0.2f;
    }

    public void EnableCurseEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.yellow;
        statusEffectTooltip[3].image.color = Color.yellow;
        CalcDamage.Instance.curseEffect10 = true;
        CalcDamage.Instance.CurseEffect10_IncreaseDebuffDamage();
    }

    public void EnableCurseEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.yellow;
        statusEffectTooltip[4].image.color = Color.yellow;
        CalcDamage.Instance.curseEffect13 = true;
    }

    public void EnableCurseEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.yellow;
        statusEffectTooltip[5].image.color = Color.yellow;
        CalcDamage.Instance.curseEffect16 = true;
    }

    public void DisableCurseEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
        playerStatus.DebuffDamage -= 0.15f;
    }

    public void DisableCurseEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
        CalcDamage.Instance.curseEffect4 = false;
    }

    public void DisableCurseEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
        CalcDamage.Instance.additionalDebuffTime -= 0.2f;
    }

    public void DisableCurseEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
        CalcDamage.Instance.curseEffect10 = false;
        CalcDamage.Instance.CurseEffect10_IncreaseDebuffDamage();
    }

    public void DisableCurseEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
        CalcDamage.Instance.curseEffect13 = false;
    }

    public void DisableCurseEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
        CalcDamage.Instance.curseEffect16 = false;
    }
}
