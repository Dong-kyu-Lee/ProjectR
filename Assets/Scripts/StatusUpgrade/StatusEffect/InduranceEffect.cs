using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InduranceEffect : MonoBehaviour
{
    private StatusEffectTooltip[] statusEffectTooltip = new StatusEffectTooltip[6];

    private void Awake()
    {
        statusEffectTooltip[0] = GameObject.Find("InduranceEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[1] = GameObject.Find("InduranceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[2] = GameObject.Find("InduranceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[3] = GameObject.Find("InduranceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[4] = GameObject.Find("InduranceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[5] = GameObject.Find("InduranceEffect16").GetComponent<StatusEffectTooltip>();
    }

    public void EnableInduranceEffect1()
    {
        statusEffectTooltip[0].defaultColor = Color.yellow;
        statusEffectTooltip[0].image.color = Color.yellow;
    }

    public void EnableInduranceEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.yellow;
        statusEffectTooltip[1].image.color = Color.yellow;
    }

    public void EnableInduranceEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.yellow;
        statusEffectTooltip[2].image.color = Color.yellow;
    }

    public void EnableInduranceEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.yellow;
        statusEffectTooltip[3].image.color = Color.yellow;
    }

    public void EnableInduranceEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.yellow;
        statusEffectTooltip[4].image.color = Color.yellow;
    }

    public void EnableInduranceEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.yellow;
        statusEffectTooltip[5].image.color = Color.yellow;
    }

    public void DisableInduranceEffect1()
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
    }

    public void DisableInduranceEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
    }

    public void DisableInduranceEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
    }

    public void DisableInduranceEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
    }

    public void DisableInduranceEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
    }

    public void DisableInduranceEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
    }
}
