using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void EnableCurseEffect1()
    {
        statusEffectTooltip[0].defaultColor = Color.yellow;
        statusEffectTooltip[0].image.color = Color.yellow;
    }

    public void EnableCurseEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.yellow;
        statusEffectTooltip[1].image.color = Color.yellow;
    }

    public void EnableCurseEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.yellow;
        statusEffectTooltip[2].image.color = Color.yellow;
    }

    public void EnableCurseEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.yellow;
        statusEffectTooltip[3].image.color = Color.yellow;
    }

    public void EnableCurseEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.yellow;
        statusEffectTooltip[4].image.color = Color.yellow;
    }

    public void EnableCurseEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.yellow;
        statusEffectTooltip[5].image.color = Color.yellow;
    }

    public void DisableCurseEffect1()
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
    }

    public void DisableCurseEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
    }

    public void DisableCurseEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
    }

    public void DisableCurseEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
    }

    public void DisableCurseEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
    }

    public void DisableCurseEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
    }
}
