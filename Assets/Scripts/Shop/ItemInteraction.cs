using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemInteraction : MonoBehaviour
{
    public Inventory inventory;
    private ShopManager shopManager;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "ShopScene")
        {
            shopManager = FindObjectOfType<ShopManager>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var itemExplain = GetComponent<ItemExplain>();
            if (itemExplain && itemExplain.IsActive())
            {
                string currentScene = SceneManager.GetActiveScene().name;
                if (currentScene == "ShopScene")
                {
                    shopManager.BuyItem(itemExplain.item);
                    itemExplain.HideUI();
                }
                else
                {
                    BasicItemData item = itemExplain.item;
                    if (item == null) return;

                    if (item.ItemType == ItemType.CONSUMABLE)
                    {
                        ConsumableItemData consumableItem = item as ConsumableItemData;
                        switch (consumableItem.kind)
                        {
                            case ConsumableKind.POTION:
                                // 포션일 경우 즉시 효과 적용
                                PlayerStatus player = FindObjectOfType<PlayerStatus>();
                                consumableItem.ActivateItemEffect(player);
                                Debug.Log($"[ItemInteraction] 포션 사용됨: {item.ItemName}");
                                break;

                            case ConsumableKind.Throwable:
                            case ConsumableKind.ETC:
                                // 나머지는 인벤토리에 추가
                                inventory.AddItem(item);
                                break;
                        }
                    }
                    else
                    {
                        inventory.AddItem(item); // 장비 아이템 등
                    }

                    itemExplain.HideUI();
                    Destroy(gameObject); // 먹은 아이템 제거
                }
            }
        }
    }
}
