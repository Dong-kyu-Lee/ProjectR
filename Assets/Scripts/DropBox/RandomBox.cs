using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 드롭 확률 테이블 클래스

[System.Serializable]
public class BoxGradeChance
{
    public BoxGrade grade;
    [Range(0f, 1f)]
    public float probability;
}

[System.Serializable]
public class ItemGradeChance
{
    public ItemGrade grade;
    [Range(0f, 1f)]
    public float probability;
}

[System.Serializable]
public class BoxGradeDropTable
{
    public BoxGrade boxGrade;
    public List<ItemGradeChance> itemGradeChances;
}

#endregion

public class RandomBox : MonoBehaviour
{
    [Header("박스 자체 등급 확률")]
    [SerializeField]
    private List<BoxGradeChance> boxGradeChances;

    [Header("박스 → 아이템 등급 확률")]
    [SerializeField]
    private List<BoxGradeDropTable> dropTables;

    private BoxGrade currentBoxGrade;

    [Header("아이템 풀")]
    [SerializeField]
    private ItemGradeList itemGradeList;

    [Header("드롭 설정")]
    [SerializeField]
    private GameObject dropItemPrefab;
    [SerializeField]
    private Transform dropParent;
    [SerializeField]
    private int minDropCount = 3;
    [SerializeField]
    private int maxDropCount = 4;
    [SerializeField]
    private float itemSpacing = 1.0f;

    private bool isOpen;
    private bool canOpen;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        isOpen = false;
        canOpen = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        DetermineBoxGrade();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen && canOpen)
            {
                DropItem();
                isOpen = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpen = false;
        }
    }

    private void DetermineBoxGrade()
    {
        float rand = Random.value;
        float cumulative = 0f;

        foreach (var entry in boxGradeChances)
        {
            cumulative += entry.probability;
            if (rand <= cumulative)
            {
                currentBoxGrade = entry.grade;
                break;
            }
        }

        switch (currentBoxGrade)
        {
            case BoxGrade.Normal: spriteRenderer.color = Color.gray; break;
            case BoxGrade.Rare: spriteRenderer.color = Color.blue; break;
            case BoxGrade.Epic: spriteRenderer.color = Color.magenta; break;
            case BoxGrade.Unique: spriteRenderer.color = Color.yellow; break;
            case BoxGrade.Legendary: spriteRenderer.color = Color.red; break;
        }
    }

    public void DropItem()
    {
        int dropNum = Random.Range(minDropCount, maxDropCount + 1);

        for (int i = 0; i < dropNum; i++)
        {
            // 1. 아이템 등급 결정
            ItemGrade selectedGrade = GetRandomItemGrade(currentBoxGrade);

            // 2. 아이템 풀에서 해당 등급 리스트 중 랜덤 선택
            BasicItemData randomItem = GetRandomItemFromGrade(selectedGrade);
            if (randomItem == null)
            {
                Debug.LogWarning($"[RandomBox] {selectedGrade} 등급 아이템 풀이 비어있습니다.");
                continue;
            }

            Vector3 dropPosition = new Vector3(
                transform.position.x + (i - (dropNum - 1) / 2f) * itemSpacing,
                dropParent.position.y - 0.5f,
                transform.position.z
            );

            // 포션이면 즉시 효과
            if (randomItem is PotionItemData potionItem)
            {
                Debug.Log($"[RandomBox] 포션 즉시 효과 적용됨: {potionItem.ItemName}");
                PlayerStatus player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
                potionItem.ActivateItemEffect(player);
                continue;
            }

            // 아이템 프리팹 드롭
            GameObject droppedItem = Instantiate(dropItemPrefab, dropPosition, Quaternion.identity, dropParent);
            ItemExplain itemExplain = droppedItem.GetComponent<ItemExplain>();
            if (itemExplain)
            {
                itemExplain.item = randomItem;
                itemExplain.ChangeInfo();
            }
        }
    }

    private ItemGrade GetRandomItemGrade(BoxGrade boxGrade)
    {
        var table = dropTables.Find(t => t.boxGrade == boxGrade);
        if (table == null || table.itemGradeChances.Count == 0)
        {
            Debug.LogWarning($"[RandomBox] {boxGrade}에 대한 아이템 등급 드롭 테이블이 없습니다.");
            return ItemGrade.Normal;
        }

        float rand = Random.value;
        float cumulative = 0f;

        foreach (var chance in table.itemGradeChances)
        {
            cumulative += chance.probability;
            if (rand <= cumulative)
            {
                return chance.grade;
            }
        }

        return ItemGrade.Normal;
    }

    private BasicItemData GetRandomItemFromGrade(ItemGrade grade)
    {
        List<BasicItemData> list = itemGradeList.GetListByGrade(grade);
        if (list == null || list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }
}
