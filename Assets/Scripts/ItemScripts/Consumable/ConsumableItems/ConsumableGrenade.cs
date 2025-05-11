using UnityEngine;

[CreateAssetMenu(fileName = "New Grenade Item Data", menuName = "Scriptable Object/Consumable Grenade Item Data", order = 1)]
public class ConsumableGrenade : ConsumableItemData
{
    //수류탄projectile 프리펩
    [SerializeField]
    private GameObject grenadePrefab;  //다이너마이트 Projectile Prefab

    public override void ActivateItemEffect(PlayerStatus player)
    {
        ThrowGrenade(player.transform);
    }

    private void ThrowGrenade(Transform playerTf)
    {
        //수류탄 프리펩 생성 후 AddForce로 던지기
        GameObject obj = Instantiate(grenadePrefab, playerTf.position, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(playerTf.forward.x, 1.0f) * 0.5f, ForceMode2D.Impulse);
    }
}
