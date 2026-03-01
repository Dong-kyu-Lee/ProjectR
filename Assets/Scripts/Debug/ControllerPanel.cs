using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControllerPanel : MonoBehaviour
{
    [Header("Links")]
    public GameObject levelGroup;
    public Text levelText;
    public Button levelMinusBtn, levelPlusBtn;

    public Button dmgMinusBtn, dmgPlusBtn;
    public Button mvMinusBtn, mvPlusBtn;
    public Button asMinusBtn, asPlusBtn;
    public Button drMinusBtn, drPlusBtn;
    public Button healBtn, resetBtn;

    private Status baseStatus;
    private PlayerStatus player;

    public void Bind(Status playerStatus)
    {
        baseStatus = playerStatus;
        player = playerStatus as PlayerStatus;

        if (levelGroup) levelGroup.SetActive(player != null);

        if (player != null)
        {
            levelMinusBtn.onClick.AddListener(() => LevelDelta(-1));
            levelPlusBtn.onClick.AddListener(() => LevelDelta(+1));
        }

        dmgMinusBtn.onClick.AddListener(() => { baseStatus.Damage = Mathf.Max(0, baseStatus.Damage - 1f); });
        dmgPlusBtn.onClick.AddListener(() => { baseStatus.Damage += 1f; });

        mvMinusBtn.onClick.AddListener(() => { baseStatus.AdditionalMoveSpeed -= 0.1f; });
        mvPlusBtn.onClick.AddListener(() => { baseStatus.AdditionalMoveSpeed += 0.1f; });

        asMinusBtn.onClick.AddListener(() => { baseStatus.AdditionalAttackSpeed -= 0.1f; });
        asPlusBtn.onClick.AddListener(() => { baseStatus.AdditionalAttackSpeed += 0.1f; });

        drMinusBtn.onClick.AddListener(() => { baseStatus.DamageReduction = Mathf.Max(0f, baseStatus.DamageReduction - 0.05f); });
        drPlusBtn.onClick.AddListener(() => { baseStatus.DamageReduction = Mathf.Min(0.9f, baseStatus.DamageReduction + 0.05f); });

        healBtn.onClick.AddListener(() => { baseStatus.Hp = baseStatus.MaxHp; });
        resetBtn.onClick.AddListener(ResetToDefaults);
    }

    void Update()
    {
        if (player != null && levelText)
            levelText.text = $"Level : {player.Level:0}";
    }

    private void LevelDelta(int d)
    {
        player.Level = Mathf.Max(1, player.Level + d);
        baseStatus.MaxHp = Mathf.Max(1, baseStatus.MaxHp + 5 * d);
        baseStatus.Hp = Mathf.Min(baseStatus.Hp, baseStatus.MaxHp);
        baseStatus.Damage = Mathf.Max(0, baseStatus.Damage + 2 * d);
    }

    private void ResetToDefaults()
    {
        baseStatus.AdditionalMoveSpeed = 0f;
        baseStatus.AdditionalAttackSpeed = 0f;
        baseStatus.DamageReduction = 0f;
    }
}
