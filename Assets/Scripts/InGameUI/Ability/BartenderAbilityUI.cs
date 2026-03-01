using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BartenderAbilityUI : AbilityUIBase
{
    [SerializeField] private Text bottleCountText;
    [SerializeField] private Text bottleNameText;
    [SerializeField] private Image bottleImage;

    private BartenderAbilityV2 bartenderAbility;

    public override void BindAbility(IAbilityV2 ability)
    {
        bartenderAbility = ability as BartenderAbilityV2;

        if (bartenderAbility != null)
        {
            bartenderAbility.onAbilityUpdated.AddListener(UpdateUI);
            UpdateUI();
        }
    }

    public override void UpdateUI()
    {
        if (bartenderAbility == null) return;

        int counts = bartenderAbility.GetBottleCounts();
        string bottleName = bartenderAbility.GetFrontBottleName();

        // 표시할 병이 없는 경우
        bottleNameText.text = bottleName;
        bottleCountText.text = counts.ToString();
    }

    public void AddBottles()
    {
        bartenderAbility?.Activate();
        UpdateUI();
    }
}
