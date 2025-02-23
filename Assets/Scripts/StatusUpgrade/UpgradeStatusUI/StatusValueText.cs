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
    public Text statpointText;

    // 스테이터스 값 표시.
    public void SetupValueText(UpgradeStatus upgradeStatus)
    {
        forceText.text = "" + upgradeStatus.Force;
        induranceText.text = "" + upgradeStatus.Indurance;
        criticalText.text = "" + upgradeStatus.Critical;
        dexterityText.text = "" + upgradeStatus.Dexterity;
        mysteryText.text = "" + upgradeStatus.Mystery;
        statpointText.text = "스탯포인트 : " + upgradeStatus.StatPoint;
    }
}
