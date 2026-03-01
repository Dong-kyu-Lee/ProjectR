using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerObj;

public class DexterityEffect : MonoBehaviour
{
    private StatusEffectTooltip[] statusEffectTooltip = new StatusEffectTooltip[6];

    private void Awake()
    {
        statusEffectTooltip[0] = transform.Find("DexterityEffect1").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[1] = transform.Find("DexterityEffect4").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[2] = transform.Find("DexterityEffect7").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[3] = transform.Find("DexterityEffect10").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[4] = transform.Find("DexterityEffect13").GetComponent<StatusEffectTooltip>();
        statusEffectTooltip[5] = transform.Find("DexterityEffect16").GetComponent<StatusEffectTooltip>();
    }

    public void EnableDexterityEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.white;
        statusEffectTooltip[0].image.color = Color.white;
        playerStatus.AdditionalAttackSpeed += 0.1f;
    }

    public void EnableDexterityEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.white;
        statusEffectTooltip[1].image.color = Color.white;
        CalcDamage.Instance.dexterityEffect4 = true;
    }

    public void EnableDexterityEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.white;
        statusEffectTooltip[2].image.color = Color.white;
        CalcDamage.Instance.dexterityEffect7 = true;
    }

    public void EnableDexterityEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.white;
        statusEffectTooltip[3].image.color = Color.white;
        CalcDamage.Instance.dexterityEffect10 = true;
    }

    public void EnableDexterityEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.white;
        statusEffectTooltip[4].image.color = Color.white;
        CalcDamage.Instance.dexterityEffect13 = true;
    }

    public void EnableDexterityEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.white;
        statusEffectTooltip[5].image.color = Color.white;
        CalcDamage.Instance.dexterityEffect16 = true;
    }

    public void DisableDexterityEffect1(PlayerStatus playerStatus)
    {
        statusEffectTooltip[0].defaultColor = Color.grey;
        statusEffectTooltip[0].image.color = Color.grey;
        playerStatus.AdditionalAttackSpeed -= 0.1f;
    }

    public void DisableDexterityEffect4()
    {
        statusEffectTooltip[1].defaultColor = Color.grey;
        statusEffectTooltip[1].image.color = Color.grey;
        CalcDamage.Instance.dexterityEffect4 = false;
    }

    public void DisableDexterityEffect7()
    {
        statusEffectTooltip[2].defaultColor = Color.grey;
        statusEffectTooltip[2].image.color = Color.grey;
        CalcDamage.Instance.dexterityEffect7 = false;
        CalcDamage.Instance.ResetBuff();

    }

    public void DisableDexterityEffect10()
    {
        statusEffectTooltip[3].defaultColor = Color.grey;
        statusEffectTooltip[3].image.color = Color.grey;
        CalcDamage.Instance.dexterityEffect10 = false;
    }

    public void DisableDexterityEffect13()
    {
        statusEffectTooltip[4].defaultColor = Color.grey;
        statusEffectTooltip[4].image.color = Color.grey;
        CalcDamage.Instance.dexterityEffect13 = false;
        CalcDamage.Instance.ResetBuff();
    }

    public void DisableDexterityEffect16()
    {
        statusEffectTooltip[5].defaultColor = Color.grey;
        statusEffectTooltip[5].image.color = Color.grey;
        CalcDamage.Instance.dexterityEffect16 = false;
        CalcDamage.Instance.dexterityEffect16_Stack = 0;
    }
}
