using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeOnClick : MonoBehaviour
{
    public void IncreaseForceOnClick(UpgradeStatus upgradeStatus)
    {
        upgradeStatus.IncreaseStat("force");
    }

    public void IncreaseSPOnClick(UpgradeStatus upgradeStatus)
    {
        upgradeStatus.SkillPoint++;
    }

    public void ResetStatOnClick(UpgradeStatus upgradeStatus)
    {
        upgradeStatus.ResetStat();
    }
}
