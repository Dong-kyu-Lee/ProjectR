using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePanelBinder : MonoBehaviour
{
    public TMP_Text pointText;
    public Button powerPlus, endurancePlus, agilityPlus, critPlus, cursePlus; // 예시
    private UpgradeStatus upg;
    private Status p;

    public void Bind(GameObject player)
    {
        upg = player.GetComponent<UpgradeStatus>();
        p = player.GetComponent<Status>();

        powerPlus.onClick.AddListener(() => SpendPoint(() => { p.Damage += 1f; }));
        endurancePlus.onClick.AddListener(() => SpendPoint(() => { p.MaxHp += 5; p.Hp = Mathf.Min(p.Hp, p.MaxHp); }));
        agilityPlus.onClick.AddListener(() => SpendPoint(() => { p.AdditionalMoveSpeed += 0.1f; }));
        cursePlus.onClick.AddListener(() => SpendPoint(() => { p.DamageReduction += 0.02f; }));

        Refresh();
    }

    private void SpendPoint(System.Action apply)
    {
        apply?.Invoke();
        Refresh();
    }

    private void Refresh()
    {
    }
}
