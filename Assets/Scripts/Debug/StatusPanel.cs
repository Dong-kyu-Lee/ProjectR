using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    public Text title;

    // 공통
    public Text hp, damage, dr, atkSpd, mvSpd;

    // 플레이어 전용
    public GameObject playerOnlyGroup;
    public Text level, crit, addDmg, gold;

    // 적 전용
    public GameObject enemyOnlyGroup;
    public Text enemyName, attackRange, sightRange, expValue;

    private Status target;
    private PlayerStatus p;
    private EnemyStatus e;

    public enum PanelMode { Auto, Player, Enemy }
    private PanelMode mode = PanelMode.Auto;

    public void Bind(Status status, string titleText, PanelMode mode = PanelMode.Auto)
    {
        this.target = status;
        this.p = status as PlayerStatus;
        this.e = status as EnemyStatus;
        this.mode = mode;

        if (title) title.text = titleText;

        var resolved = mode == PanelMode.Auto
            ? (p != null ? PanelMode.Player : PanelMode.Enemy)
            : mode;

        bool isPlayer = resolved == PanelMode.Player;
        if (playerOnlyGroup) playerOnlyGroup.SetActive(isPlayer);
        if (enemyOnlyGroup) enemyOnlyGroup.SetActive(!isPlayer);

        Refresh();
    }

    void Update()
    {
        if (!target) return;
        Refresh();
    }

    private void Refresh()
    {
        // 공통
        if (hp) hp.text = $"HP : {Mathf.RoundToInt(target.Hp)}/{Mathf.RoundToInt(target.MaxHp)}";
        if (damage) damage.text = $"피해량 : {target.Damage:0.##}";
        if (dr) dr.text = $"피해 감소량 : {target.DamageReduction * 100f:0.#}%";
        if (atkSpd) atkSpd.text = $"공격속도 : {target.TotalAttackSpeed:0.##}";
        if (mvSpd) mvSpd.text = $"이동속도 : {target.TotalMoveSpeed:0.##}";

        // 플레이어 전용
        if (p != null)
        {
            if (level) level.text = $"레벨 : {p.Level:0}";
            if (crit) crit.text = $"치명타 확률 : {p.CriticalPercent * 100f:0.#}%";
            if (addDmg) addDmg.text = $"추가 피해량 : {p.AdditionalDamage:0.##}";
            if (gold) gold.text = $"보유 골드 : {p.Gold:0}";
        }

        // 적 전용
        if (e != null && e.EnemyStatusData != null)
        {
            var d = e.EnemyStatusData;
            if (enemyName) enemyName.text = $"이름 : {d.Name}";
            if (attackRange) attackRange.text = $"사거리 : {d.AttackRange:0.##}";
            if (sightRange) sightRange.text = $"시야 : {d.SightRange:0.##}";
            if (expValue) expValue.text = $"EXP : {d.ExpValue:0}";
        }
    }
}
