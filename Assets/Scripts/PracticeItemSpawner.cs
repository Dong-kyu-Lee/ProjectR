using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeItemSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField, Tooltip("스폰될 위치 (자식 오브젝트나 현재 위치를 지정하세요)")]
    private Transform spawnPoint;

    [SerializeField, Tooltip("필드에 생성할 투척 아이템 프리팹 (예: ItemSlot_ForThrowableItem)")]
    private GameObject itemPrefab;

    [SerializeField, Tooltip("랜덤으로 스폰될 투척 아이템 데이터 리스트")]
    private List<ConsumableItemData> throwableItemDatas;

    [SerializeField, Tooltip("아이템 획득 후 다시 스폰될 때까지의 대기 시간 (초)")]
    private float respawnDelay = 2.0f;

    // 현재 스폰되어 있는 아이템 오브젝트 추적용
    private GameObject currentSpawnedItem;
    private bool isRespawning = false;

    private void Start()
    {
        if (spawnPoint == null) spawnPoint = transform; // 지정 안 하면 자기 자신 위치로

        SpawnRandomItem();
    }

    private void Update()
    {
        // 필드에 스폰된 아이템이 파괴되었는지(플레이어가 먹었는지) 감지
        if (currentSpawnedItem == null && !isRespawning)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private void SpawnRandomItem()
    {
        if (itemPrefab == null || throwableItemDatas == null || throwableItemDatas.Count == 0)
        {
            Debug.LogWarning("[PracticeItemSpawner] 프리팹이나 아이템 데이터 리스트가 비어있습니다.");
            return;
        }

        // 랜덤 아이템 뽑기
        int randomIndex = Random.Range(0, throwableItemDatas.Count);
        ConsumableItemData selectedData = throwableItemDatas[randomIndex];

        // 프리팹 생성
        currentSpawnedItem = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);

        // 생성된 프리팹에 데이터 덮어씌우기 (ItemExplain 연동)
        ItemExplain explainComp = currentSpawnedItem.GetComponent<ItemExplain>();
        if (explainComp != null)
        {
            explainComp.item = selectedData;
            explainComp.ChangeInfo(); // 스프라이트, 이름 등 UI 정보 즉시 갱신
        }
        else
        {
            Debug.LogWarning("[PracticeItemSpawner] 프리팹에 ItemExplain 컴포넌트가 없습니다.");
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        // 2초 대기
        yield return new WaitForSeconds(respawnDelay);

        SpawnRandomItem();
        isRespawning = false;
    }
}