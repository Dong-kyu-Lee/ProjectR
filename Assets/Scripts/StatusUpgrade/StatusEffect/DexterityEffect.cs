using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DexterityEffect : MonoBehaviour
{
    private StatusEffectTooltip statusEffectTooltip;

    public void EnableDexterityEffect1()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableDexterityEffect4()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableDexterityEffect7()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableDexterityEffect10()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableDexterityEffect13()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void EnableDexterityEffect16()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.yellow;
        statusEffectTooltip.image.color = Color.yellow;
    }

    public void DisableDexterityEffect1()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableDexterityEffect4()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableDexterityEffect7()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableDexterityEffect10()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableDexterityEffect13()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }

    public void DisableDexterityEffect16()
    {
        statusEffectTooltip = GameObject.Find("DexterityEffect16").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip.defaultColor = Color.white;
        statusEffectTooltip.image.color = Color.white;
    }
}
