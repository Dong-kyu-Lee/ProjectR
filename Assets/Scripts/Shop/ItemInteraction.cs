using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemInteraction : MonoBehaviour
{
    public Inventory inventory;
    private ShopManager shopManager;
    public bool isShop;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "ShopScene")
        {
            shopManager = FindObjectOfType<ShopManager>();
            isShop = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var itemExplain = GetComponent<ItemExplain>();
            if (itemExplain && itemExplain.IsActive())
            {
                if (isShop)
                {
                    shopManager.BuyItem(itemExplain.item);
                    itemExplain.HideUI();
                }
                else
                {
                    BasicItemData item = itemExplain.item;
                    if (item == null) return;

                    bool added = false;

                    if (item.ItemType == ItemType.CONSUMABLE)
                    {
                        ConsumableItemData consumableItem = item as ConsumableItemData;
                        switch (consumableItem.kind)
                        {
                            case ConsumableKind.Throwable:
                            case ConsumableKind.ETC:
                                added = inventory.AddItem(item);
                                break;
                        }
                    }
                    else
                    {
                        added = inventory.AddItem(item); // 장비 아이템 등
                    }

                    // 인벤토리에 정상적으로 추가되었을 때만 필드에서 제거
                    if (added)
                    {
                        itemExplain.HideUI();
                        Destroy(gameObject);
                    }
                    else
                    {
                        //실패
                        return;
                    }
                }

            }
        }
    }
}
