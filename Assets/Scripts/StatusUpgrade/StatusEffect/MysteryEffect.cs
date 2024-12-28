using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryEffect : MonoBehaviour
{
    private StatusEffectTooltip[] statusEffectTooltip = new StatusEffectTooltip[6];

    private void Awake()
    {
        statusEffectTooltip[0] = GameObject.Find("MysteryEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[1] = GameObject.Find("MysteryEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[2] = GameObject.Find("MysteryEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[3] = GameObject.Find("MysteryEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[4] = GameObject.Find("MysteryEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[5] = GameObject.Find("MysteryEffect16").GetComponent<StatusEffectTooltip>();
    }

    public void EnableMysteryEffect1()
    {
        statusEffectTooltip[0].defaultColor = Color.yellow;
        statusEffectTooltip[0].image.color = Color.yellow;
    }

    public void EnableMysteryEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.yellow;
        statusEffectTooltip[1].image.color = Color.yellow;
    }

    public void EnableMysteryEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.yellow;
        statusEffectTooltip[2].image.color = Color.yellow;
    }

    public void EnableMysteryEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.yellow;
        statusEffectTooltip[3].image.color = Color.yellow;
    }

    public void EnableMysteryEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.yellow;
        statusEffectTooltip[4].image.color = Color.yellow;
    }

    public void EnableMysteryEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.yellow;
        statusEffectTooltip[5].image.color = Color.yellow;
    }

    public void DisableMysteryEffect1()
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
    }

    public void DisableMysteryEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
    }

    public void DisableMysteryEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
    }

    public void DisableMysteryEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
    }

    public void DisableMysteryEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
    }

    public void DisableMysteryEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
    }
}
