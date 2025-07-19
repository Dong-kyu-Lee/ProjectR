using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Item/Potion")]
public class PotionItemData : ConsumableItemData
{
    public PotionType potionType;

    [SerializeField] public BuffType linkedBuffType;
    [SerializeField] public float effectValue = 10f;

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        if (linkedBuffType == BuffType.None)
        {
            float maxHp = playerStatus.MaxHp;
            float heal = maxHp * (effectValue / 100f);
            playerStatus.Hp += heal;
            if (playerStatus.Hp > maxHp)
            {
                playerStatus.Hp = maxHp;
            }
            Debug.Log($"[PotionItemData] HP {effectValue}% 회복 → +{heal}");
            return;
        }

        BuffManager buffManager = GameObject.FindObjectOfType<BuffManager>();
        if (buffManager != null)
        {
            buffManager.ActivateBuff(linkedBuffType, 10000f);
            Debug.Log($"[PotionItemData] 버프 포션 사용: {linkedBuffType}");
        }
        else
        {
            Debug.LogError("[PotionItemData] BuffManager가 없습니다.");
        }
    }
}
