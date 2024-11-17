using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InduranceEffect : MonoBehaviour
{
    private StatusEffectTooltip statusEffectTooltip;

    public void EnableInduranceEffect1()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableInduranceEffect4()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableInduranceEffect7()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableInduranceEffect10()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableInduranceEffect13()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableInduranceEffect16()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void DisableInduranceEffect1()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableInduranceEffect4()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableInduranceEffect7()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableInduranceEffect10()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableInduranceEffect13()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableInduranceEffect16()
    {
        statusEffectTooltip = GameObject.Find("InduranceEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }
}
