using UnityEngine;

[CreateAssetMenu(fileName = "New MoveSpeedPotion", menuName = "Item/Potion/MoveSpeed")]
public class MoveSpeedPotion : PotionItemData
{
    [SerializeField] private float moveSpeedIncreasement = 10f;
    public float MoveSpeedIncreasement => moveSpeedIncreasement;

    private void OnEnable()
    {
        kind = ConsumableKind.POTION;
        potionType = PotionType.MoveSpeedIncrease;
    }

    public override void ActivateItemEffect(PlayerStatus playerStatus)
    {
        float before = playerStatus.MoveSpeed;
        playerStatus.MoveSpeed += playerStatus.MoveSpeed * moveSpeedIncreasement / 100f;
        Debug.Log($"[MoveSpeedPotion] 이동속도 {moveSpeedIncreasement}% 증가: {before} → {playerStatus.MoveSpeed}");
    }
}
