using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryEffect : MonoBehaviour
{
    private StatusEffectTooltip statusEffectTooltip;

    public void EnableMysteryEffect1()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableMysteryEffect4()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableMysteryEffect7()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableMysteryEffect10()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableMysteryEffect13()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableMysteryEffect16()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void DisableMysteryEffect1()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableMysteryEffect4()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableMysteryEffect7()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableMysteryEffect10()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableMysteryEffect13()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableMysteryEffect16()
    {
        statusEffectTooltip = GameObject.Find("MysteryEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }
}
