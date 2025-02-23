using System.Collections;
using System.Collections.Generic;
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
                    inventory.AddItem(itemExplain.item);
                    itemExplain.HideUI();
                    Destroy(gameObject);
                }
            }
        }
    }
}
