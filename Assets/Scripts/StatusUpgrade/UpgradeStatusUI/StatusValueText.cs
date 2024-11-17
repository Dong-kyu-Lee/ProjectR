using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusValueText : MonoBehaviour
{
    public Text forceText;
    public Text induranceText;
    public Text criticalText;
    public Text dexterityText;
    public Text mysteryText;
    public Text skillpointText;

    public void SetupValueText(UpgradeStatus upgradeStatus)
    {
        forceText.text = "" + upgradeStatus.Force;
        induranceText.text = "" + upgradeStatus.Indurance;
        criticalText.text = "" + upgradeStatus.Critical;
        dexterityText.text = "" + upgradeStatus.Dexterity;
        mysteryText.text = "" + upgradeStatus.Mystery;
        skillpointText.text = "스킬포인트 : " + upgradeStatus.SkillPoint;
    }
}
