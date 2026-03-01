using UnityEngine;

[CreateAssetMenu(fileName = "New Grenade Item Data", menuName = "Scriptable Object/Consumable Grenade Item Data", order = 1)]
public class ConsumableGrenade : ConsumableItemData
{
    [SerializeField]
    public GameObject grenadePrefab;

    public override void ActivateItemEffect(PlayerStatus player)
    {
        player.GetComponentInChildren<GrenadeController>().StartAim(grenadePrefab);
    }
}
