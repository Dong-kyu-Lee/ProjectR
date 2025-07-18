using UnityEngine;

[CreateAssetMenu(fileName = "New AttackSpeedPotion", menuName = "Item/Potion/AttackSpeed")]
public class AttackSpeedPotion : PotionItemData
{
    [SerializeField] private float attackSpeedIncreasement = 10f;
    public float AttackSpeedIncreasement => attackSpeedIncreasement;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.AttackSpeedIncrease;
    }

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        float before = playerStatus.AdditionalAttackSpeed;
        playerStatus.AdditionalAttackSpeed += attackSpeedIncreasement / 100f;
        Debug.Log($"[AttackSpeedPotion] 공격속도 {attackSpeedIncreasement}% 증가: {before} → {playerStatus.AdditionalAttackSpeed}");
    }
}
