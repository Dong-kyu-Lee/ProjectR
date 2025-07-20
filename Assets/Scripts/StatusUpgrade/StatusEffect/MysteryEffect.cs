using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerObj;

public class MysteryEffect : MonoBehaviour
{
    private StatusEffectTooltip[] statusEffectTooltip = new StatusEffectTooltip[6];

    private void Awake()
    {
        statusEffectTooltip[0] = transform.Find("MysteryEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[1] = transform.Find("MysteryEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[2] = transform.Find("MysteryEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[3] = transform.Find("MysteryEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[4] = transform.Find("MysteryEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[5] = transform.Find("MysteryEffect16").GetComponent<StatusEffectTooltip>();
    }

    public void EnableMysteryEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.yellow;
        statusEffectTooltip[0].image.color = Color.yellow;
        playerStatus.Damage += 2;
        playerStatus.MaxHp += 10;
    }

    public void EnableMysteryEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.yellow;
        statusEffectTooltip[1].image.color = Color.yellow;
        RuneSpawner.Instance.RuneSpawnChanceUp(0.05f);
    }

    public void EnableMysteryEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.yellow;
        statusEffectTooltip[2].image.color = Color.yellow;
        CalcDamage.Instance.mysteryEffect7 = true;
    }

    public void EnableMysteryEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.yellow;
        statusEffectTooltip[3].image.color = Color.yellow;
        RuneSpawner.Instance.AddBuffType();
        RuneSpawner.Instance.RuneSpawnChanceUp(0.05f);
    }

    public void EnableMysteryEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.yellow;
        statusEffectTooltip[4].image.color = Color.yellow;
        CalcDamage.Instance.mysteryEffect13 = true;
    }

    public void EnableMysteryEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.yellow;
        statusEffectTooltip[5].image.color = Color.yellow;
        CalcDamage.Instance.mysteryEffect16 = true;
    }

    public void DisableMysteryEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
        playerStatus.Damage -= 2;
        playerStatus.MaxHp -= 10;
    }

    public void DisableMysteryEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
        RuneSpawner.Instance.RuneSpawnChanceUp(-0.05f);
    }

    public void DisableMysteryEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
        CalcDamage.Instance.mysteryEffect7 = false;

    }

    public void DisableMysteryEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
        RuneSpawner.Instance.RemoveBuffType();
        RuneSpawner.Instance.RuneSpawnChanceUp(-0.05f);
    }

    public void DisableMysteryEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
        CalcDamage.Instance.mysteryEffect13 = false;
    }

    public void DisableMysteryEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
        CalcDamage.Instance.mysteryEffect16 = false;
    }
}
