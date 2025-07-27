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
                            case ConsumableKind.Throwable:
                            case ConsumableKind.ETC:
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
