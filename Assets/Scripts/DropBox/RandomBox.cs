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

// [추가됨] 등급별 스프라이트를 연결하기 위한 클래스
[System.Serializable]
public class BoxGradeSprite
{
    public BoxGrade grade;
    public Sprite sprite;
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

    [Header("박스 등급별 이미지 설정")]
    [SerializeField]
    private List<BoxGradeSprite> boxGradeSprites;

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

                canOpen = false;
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

        // 박스 등급 결정
        foreach (var entry in boxGradeChances)
        {
            cumulative += entry.probability;
            if (rand <= cumulative)
            {
                currentBoxGrade = entry.grade;
                break;
            }
        }

        // 결정된 등급에 맞는 이미지 적용
        BoxGradeSprite matchedSprite = boxGradeSprites.Find(x => x.grade == currentBoxGrade);

        if (matchedSprite != null && matchedSprite.sprite != null)
        {
            spriteRenderer.sprite = matchedSprite.sprite;

            spriteRenderer.color = Color.white;
        }
        else
        {
            Debug.LogWarning($"등급에 해당하는 이미지가 없습니다: {currentBoxGrade}");
            spriteRenderer.color = Color.gray;
        }
    }

    public void DropItem()
    {
        int dropNum = Random.Range(minDropCount, maxDropCount + 1);

        // 박스 내 중복 방지
        HashSet<BasicItemData> alreadyDropped = new HashSet<BasicItemData>();

        // 플레이어 소유 아이템 (장비 포함)
        Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Inventory>();

        // 인벤토리가 없을 경우를 대비한 예외 처리
        HashSet<BasicItemData> ownedItems;
        if (inventory != null)
        {
            ownedItems = new HashSet<BasicItemData>(inventory.GetOwnedItems());
        }
        else
        {
            ownedItems = new HashSet<BasicItemData>();
        }

        for (int i = 0; i < dropNum; i++)
        {
            // 아이템 등급 선택
            ItemGrade selectedGrade = GetRandomItemGrade(currentBoxGrade);

            // 후보 중 하나 선택
            BasicItemData randomItem = GetFilteredRandomItem(selectedGrade, alreadyDropped, ownedItems);

            if (randomItem == null)
            {
                continue;
            }

            alreadyDropped.Add(randomItem);

            Vector3 dropPosition = new Vector3(
                transform.position.x + (i - (dropNum - 1) / 2f) * itemSpacing,
                dropParent.position.y - 0.5f,
                transform.position.z - 1f
            );

            //룬 생성 시도
            RuneSpawner.Instance.TrySpawnRune(dropPosition);

            //일반 아이템 드롭
            GameObject droppedItem = Instantiate(dropItemPrefab, dropPosition, Quaternion.identity, dropParent);
            ItemExplain itemExplain = droppedItem.GetComponent<ItemExplain>();
            if (itemExplain)
            {
                itemExplain.item = randomItem;
                itemExplain.ChangeInfo();
            }
        }
    }

    private BasicItemData GetFilteredRandomItem(
        ItemGrade grade,
        HashSet<BasicItemData> alreadySelected,
        HashSet<BasicItemData> ownedItems)
    {
        List<BasicItemData> list = itemGradeList.GetListByGrade(grade);
        if (list == null || list.Count == 0) return null;

        List<BasicItemData> candidates = new List<BasicItemData>();

        foreach (var item in list)
        {
            // 장비는 소유 중이면 드롭 안 함
            if (item is EquipmentItemData && ownedItems.Contains(item))
                continue;

            // 한 박스에서 중복 제거
            if (alreadySelected.Contains(item))
                continue;

            candidates.Add(item);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    private ItemGrade GetRandomItemGrade(BoxGrade boxGrade)
    {
        var table = dropTables.Find(t => t.boxGrade == boxGrade);
        if (table == null || table.itemGradeChances.Count == 0)
        {
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
}