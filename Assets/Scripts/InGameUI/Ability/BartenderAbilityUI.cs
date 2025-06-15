using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BartenderAbilityUI : AbilityUIBase
{
    [SerializeField] private Text[] bottleCountTexts;

    private BartenderAbilityV2 bartenderAbility;

    public override void BindAbility(IAbilityV2 ability)
    {
        bartenderAbility = ability as BartenderAbilityV2;
        UpdateUI();
    }

    public override void UpdateUI()
    {
        if (bartenderAbility == null || bottleCountTexts == null) return;

        var counts = bartenderAbility.GetBottleCounts();
        var types = bartenderAbility.BottleElements;

        for (int i = 0; i < types.Length && i < bottleCountTexts.Length; i++)
        {
            string type = types[i];
            int count = counts.ContainsKey(type) ? counts[type] : 0;
            bottleCountTexts[i].text = $"{type}: {count}";
        }
    }

    public void AddBottles()
    {
        bartenderAbility?.Activate();
        UpdateUI();
    }
}
