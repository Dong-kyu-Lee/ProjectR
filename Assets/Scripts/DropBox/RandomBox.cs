using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBox : MonoBehaviour
{
    [SerializeField]
    private List<BasicItemData> normalBox;
    [SerializeField]
    private List<BasicItemData> rareBox;
    [SerializeField]
    private List<BasicItemData> epicBox;
    [SerializeField]
    private List<BasicItemData> uniqueBox;
    [SerializeField]
    private List<BasicItemData> legendaryBox;
    [SerializeField]
    private GameObject dropItemPrefab; // 드랍할 아이템 프리펩
    [SerializeField]
    private Transform dropParent; // 아이템이 드랍될 위치의 부모 오브젝트
    [SerializeField]
    private int minDropCount = 3; // 최소 드랍 아이템 개수
    [SerializeField]
    private int maxDropCount = 4; // 최대 드랍 아이템 개수
    [SerializeField]
    private float itemSpacing = 1.0f; // 아이템 사이의 간격
    private bool isOpen = false;


    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BoxGrade();
    }

    private void BoxGrade()
    {
        int boxGrade = Random.Range(0, 5);
        switch (boxGrade)
        {
            case 0:
                spriteRenderer.color = Color.gray; // Normal
                break;
            case 1:
                spriteRenderer.color = Color.blue; // Rare
                break;
            case 2:
                spriteRenderer.color = Color.magenta; // Epic
                break;
            case 3:
                spriteRenderer.color = Color.yellow; // Unique
                break;
            case 4:
                spriteRenderer.color = Color.red; // Legendary
                break;
        }
    }

    public void DropItem()
    {
        List<BasicItemData> itemList = GetItemListByColor();
        int dropNum = Mathf.Min(Random.Range(minDropCount, maxDropCount + 1), itemList.Count); // 드랍 개수를 아이템 리스트 크기와 비교하여 설정
        HashSet<int> selectedIndices = new HashSet<int>();

        for (int i = 0; i < dropNum; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, itemList.Count);
            } while (selectedIndices.Contains(randomIndex));

            selectedIndices.Add(randomIndex);
            BasicItemData randomItem = itemList[randomIndex];

            // 아이템 드랍
            Vector3 dropPosition = transform.position + new Vector3((i - (dropNum - 1) / 2f) * itemSpacing, 0, 0);
            GameObject droppedItem = Instantiate(dropItemPrefab, dropPosition, Quaternion.identity, dropParent);
            ItemExplain itemExplain = droppedItem.GetComponent<ItemExplain>();
            if (itemExplain)
            {
                itemExplain.item = randomItem;
                itemExplain.ChangeInfo(); // 아이템 정보를 업데이트합니다.
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isOpen)
            {
                DropItem();
                isOpen = true;
            }
        }
    }

    private List<BasicItemData> GetItemListByColor()
    {
        if (spriteRenderer.color == Color.gray)
        {
            return normalBox;
        }
        else if (spriteRenderer.color == Color.blue)
        {
            return rareBox;
        }
        else if (spriteRenderer.color == Color.magenta)
        {
            return epicBox;
        }
        else if (spriteRenderer.color == Color.yellow)
        {
            return uniqueBox;
        }
        else if (spriteRenderer.color == Color.red)
        {
            return legendaryBox;
        }
        else
        {
            return normalBox;
        }
    }
}
