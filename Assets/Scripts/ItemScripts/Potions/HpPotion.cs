using UnityEngine;

[CreateAssetMenu(fileName = "New HpPotion", menuName = "Item/Potion/HpPotion")]
public class HpPotion : PotionItemData
{
    [SerializeField] private float hpIncreasePercent = 30f;
    public float HpIncreasePercent => hpIncreasePercent;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.HpIncrease;
    }
    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        float beforeHp = playerStatus.Hp;
        playerStatus.Hp += playerStatus.Hp * hpIncreasePercent / 100f;
        Debug.Log($"[HpPotion] HP 회복: {beforeHp} → {playerStatus.Hp}");
    }
}
