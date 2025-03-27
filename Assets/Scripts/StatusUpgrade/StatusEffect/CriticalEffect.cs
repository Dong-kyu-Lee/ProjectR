using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalEffect : MonoBehaviour
{
    private StatusEffectTooltip[] statusEffectTooltip = new StatusEffectTooltip[6];

    private void Awake()
    {
        statusEffectTooltip[0] = transform.Find("CriticalEffect/CriticalEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[1] = transform.Find("CriticalEffect/CriticalEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[2] = transform.Find("CriticalEffect/CriticalEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[3] = transform.Find("CriticalEffect/CriticalEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[4] = transform.Find("CriticalEffect/CriticalEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[5] = transform.Find("CriticalEffect/CriticalEffect16").GetComponent<StatusEffectTooltip>();
    }

    public void EnableCriticalEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.yellow;
        statusEffectTooltip[0].image.color = Color.yellow;
        playerStatus.CriticalDamage += 0.1f;
    }

    public void EnableCriticalEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.yellow;
        statusEffectTooltip[1].image.color = Color.yellow;
        CalcDamage.Instance.criticalEffect4 = true;
    }

    public void EnableCriticalEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.yellow;
        statusEffectTooltip[2].image.color = Color.yellow;
        CalcDamage.Instance.criticalEffect7 = true;
    }

    public void EnableCriticalEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.yellow;
        statusEffectTooltip[3].image.color = Color.yellow;
        CalcDamage.Instance.criticalEffect10 = true;
    }

    public void EnableCriticalEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.yellow;
        statusEffectTooltip[4].image.color = Color.yellow;
        CalcDamage.Instance.criticalEffect13 = true;
    }

    public void EnableCriticalEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.yellow;
        statusEffectTooltip[5].image.color = Color.yellow;
        CalcDamage.Instance.criticalEffect16 = true;
    }

    public void DisableCriticalEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
        playerStatus.CriticalDamage -= 0.1f;
    }

    public void DisableCriticalEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
        CalcDamage.Instance.criticalEffect4 = false;
        CalcDamage.Instance.ResetBuff();
    }

    public void DisableCriticalEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
        CalcDamage.Instance.criticalEffect7 = false;
        CalcDamage.Instance.ResetBuff();
    }

    public void DisableCriticalEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
        CalcDamage.Instance.criticalEffect10 = false;
    }

    public void DisableCriticalEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
        CalcDamage.Instance.criticalEffect13 = false;
    }

    public void DisableCriticalEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
        CalcDamage.Instance.criticalEffect16 = false;
    }
}
