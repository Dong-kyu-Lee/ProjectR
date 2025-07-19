using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Item/Potion")]
public class PotionItemData : ConsumableItemData
{
    public PotionType potionType;

    [SerializeField]
    public BuffType linkedBuffType;

    [SerializeField]
    public float buffDuration = 10f;

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        BuffManager buffManager = playerStatus.GetComponent<BuffManager>();
        if (buffManager != null)
        {
            buffManager.ActivateBuff(linkedBuffType, buffDuration);
            Debug.Log($"[PotionItemData] 버프 포션 사용: {linkedBuffType} ({buffDuration}초)");
        }
        else
        {
            Debug.LogError("[PotionItemData] BuffManager가 없습니다.");
        }
    }
}
