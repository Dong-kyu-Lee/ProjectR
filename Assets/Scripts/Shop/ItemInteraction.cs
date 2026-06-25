using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemInteraction : MonoBehaviour
{
    public Inventory inventory;
    private ShopManager shopManager;
    public bool isShop;

    private void Awake()
    {
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
                    // PlayerManager를 통해 현재 플레이어의 인벤토리를 가져옵니다.
                    if (inventory == null)
                    {
                        if (PlayerManager.Instance != null && PlayerManager.Instance.CurrentPlayer != null)
                        {
                            // 하이어라키 구조에 맞게 자식 오브젝트에서 Inventory 컴포넌트를 탐색
                            inventory = PlayerManager.Instance.CurrentPlayer.GetComponentInChildren<Inventory>();
                        }
                    }

                    // 연결 실패 방어 코드
                    if (inventory == null)
                    {
                        Debug.LogWarning("인벤토리를 찾을 수 없어 아이템을 획득할 수 없습니다.");
                        return;
                    }

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
                }
            }
        }
    }
}