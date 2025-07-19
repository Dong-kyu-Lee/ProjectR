using UnityEngine;

[CreateAssetMenu(fileName = "New HpPotion", menuName = "Item/Potion/HpPotion")]
public class HpPotion : PotionItemData
{
    [SerializeField] private float hpIncreasePercent = 30f;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.HpIncrease;
    }

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        float beforeHp = playerStatus.Hp;
        float healAmount = playerStatus.MaxHp * hpIncreasePercent / 100f;
        playerStatus.Hp += healAmount;
        Debug.Log($"[HpPotion] 체력 회복: {beforeHp} → {playerStatus.Hp} (+{healAmount})");
    }
}
