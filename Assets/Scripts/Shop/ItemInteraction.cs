using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemInteraction : MonoBehaviour
{
    [SerializeField]
    Inventory inventory;
    private ShopManager shopManager;
    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "ShopScene")
        {
            shopManager = FindObjectOfType<ShopManager>();
        }
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
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
                else if (currentScene == "DungeonScene")
                {
                    inventory.AddItem(itemExplain.item);
                    itemExplain.HideUI();
                    Destroy(gameObject);
                }
            }
        }
    }
}
