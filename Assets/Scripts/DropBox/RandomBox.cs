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

[System.Serializable]
public class BoxGradeSprite
{
    public BoxGrade grade;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;
}

#endregion

public class RandomBox : MonoBehaviour
{
    [Header("비주얼 및 참조")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("박스 등급별 이미지 & 애니메이션")]
    [SerializeField]
    private List<BoxGradeSprite> boxGradeSprites;

    [Header("박스 등급 확률")]
    [SerializeField]
    private List<BoxGradeChance> boxGradeChances;

    [Header("아이템 등급 확률 & 아이템 풀")]
    [SerializeField]
    private List<BoxGradeDropTable> dropTables;
    [SerializeField]
    private ItemGradeList itemGradeList;

    [Header("드롭 위치 및 개수 설정")]
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

    // 드랍된 아이템들을 추적/관리하는 리스트
    private List<GameObject> spawnedItems = new List<GameObject>();

    // 내부 변수
    private BoxGrade currentBoxGrade;
    private bool canOpen = false;
    private bool isOpened;

    public bool IsOpened
    {
        get { return isOpened; }
        set
        {
            isOpened = value;
            if (animator != null)
            {
                animator.SetBool("IsOpened", isOpened);
            }
        }
    }

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();

        IsOpened = false;
        canOpen = false;

        DetermineBoxGrade(); // 등급 결정 및 애니메이션 교체
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!IsOpened && canOpen)
            {
                Open();
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        spawnedItems.Clear();
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

    public void Open()
    {
        if (IsOpened) return;

        IsOpened = true; // 애니메이션 재생
        DropItem();      // 아이템 생성
    }

    //박스 등급 결정 및 애니메이션 교체 로직
    private void DetermineBoxGrade()
    {
        float rand = Random.value;
        float cumulative = 0f;

        //등급 결정
        foreach (var entry in boxGradeChances)
        {
            cumulative += entry.probability;
            if (rand <= cumulative)
            {
                currentBoxGrade = entry.grade;
                break;
            }
        }

        //등급에 맞는 데이터 찾기
        BoxGradeSprite matchedSprite = boxGradeSprites.Find(x => x.grade == currentBoxGrade);

        if (matchedSprite != null)
        {
            if (matchedSprite.sprite != null)
            {
                spriteRenderer.sprite = matchedSprite.sprite;
                spriteRenderer.color = Color.white;
            }

            // 등급별 애니메이터 교체
            if (matchedSprite.animatorController != null)
            {
                animator.runtimeAnimatorController = matchedSprite.animatorController;
            }
        }
    }

    public void DropItem()
    {
        int dropNum = Random.Range(minDropCount, maxDropCount + 1);
        HashSet<BasicItemData> alreadyDropped = new HashSet<BasicItemData>();

        Inventory inventory = null;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            inventory = player.GetComponentInChildren<Inventory>();

        HashSet<BasicItemData> ownedItems;
        if (inventory != null)
            ownedItems = new HashSet<BasicItemData>(inventory.GetOwnedItems());
        else
            ownedItems = new HashSet<BasicItemData>();

        for (int i = 0; i < dropNum; i++)
        {
            ItemGrade selectedGrade = GetRandomItemGrade(currentBoxGrade);
            BasicItemData randomItem = GetFilteredRandomItem(selectedGrade, alreadyDropped, ownedItems);

            if (randomItem == null) continue;

            alreadyDropped.Add(randomItem);

            //아이템 생성 위치
            Vector3 dropPosition = new Vector3(
                transform.position.x + (i - (dropNum - 1) / 2f) * itemSpacing,
                (dropParent != null ? dropParent.position.y : transform.position.y) + 0.5f,
                transform.position.z - 1f
            );

            if (RuneSpawner.Instance != null)
                RuneSpawner.Instance.TrySpawnRune(dropPosition);

            if (dropItemPrefab != null)
            {
                GameObject droppedItem = Instantiate(dropItemPrefab, dropPosition, Quaternion.identity, null);

                spawnedItems.Add(droppedItem);

                ItemExplain itemExplain = droppedItem.GetComponent<ItemExplain>();
                if (itemExplain)
                {
                    itemExplain.item = randomItem;
                    itemExplain.ChangeInfo();
                }
            }
        }
    }

    private BasicItemData GetFilteredRandomItem(ItemGrade grade, HashSet<BasicItemData> alreadySelected, HashSet<BasicItemData> ownedItems)
    {
        if (itemGradeList == null) return null;

        List<BasicItemData> list = itemGradeList.GetListByGrade(grade);
        if (list == null || list.Count == 0) return null;

        List<BasicItemData> candidates = new List<BasicItemData>();

        foreach (var item in list)
        {
            if (item is EquipmentItemData && ownedItems.Contains(item))
                continue;
            if (alreadySelected.Contains(item))
                continue;

            candidates.Add(item);
        }

        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }

    private ItemGrade GetRandomItemGrade(BoxGrade boxGrade)
    {
        var table = dropTables.Find(t => t.boxGrade == boxGrade);
        if (table == null || table.itemGradeChances.Count == 0)
            return ItemGrade.Normal;

        float rand = Random.value;
        float cumulative = 0f;

        foreach (var chance in table.itemGradeChances)
        {
            cumulative += chance.probability;
            if (rand <= cumulative)
                return chance.grade;
        }
        return ItemGrade.Normal;
    }
}