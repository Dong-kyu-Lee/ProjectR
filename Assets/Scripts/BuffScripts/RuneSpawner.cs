using System.Collections.Generic;
using UnityEngine;

public class RuneSpawner : MonoBehaviour
{
    public GameObject runePrefab; // 룬 프리팹
    private float spawnChance = 0.05f; // 룬 생성 확률

    private static RuneSpawner instance;

    public static RuneSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject runeSpawnerObject = new GameObject("RuneSpawner");
                instance = runeSpawnerObject.AddComponent<RuneSpawner>();
                DontDestroyOnLoad(runeSpawnerObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            runePrefab = Resources.Load<GameObject>("Prefabs/Buff,Debuff/RunePrefab");
        }

        else if (instance != this)
        {
            Destroy(gameObject); // 중복된 RuneSpawner 제거
        }
    }

    private void Update()
    {
        if (CalcDamage.Instance.mysteryEffect7 && !CalcDamage.Instance.IsOnCooldown("MysteryEffect7")) MysteryEffect7_GetBuff();
    }

    // 신비 7레벨 랜덤 버프 효과 획득
    private void MysteryEffect7_GetBuff()
    {
        BuffType buffType = GetRandomBuffType();

        BuffManager playerBuffManager = GameManager.Instance.CurrentPlayer.GetComponent<BuffManager>();
        playerBuffManager.ActivateBuff(buffType, 60f);

        CalcDamage.Instance.StartCooldown("MysteryEffect7", 30f);
    }

    // 기본 버프 타입들.
    private List<BuffType> buffTypes = new List<BuffType>
    {
        BuffType.AttackDamageIncrease,
        BuffType.DamageReductionIncrease,
        BuffType.CritPercentIncrease,
        BuffType.CritDamageIncrease,
        BuffType.AttackSpeedIncrease,
        BuffType.MoveSpeedIncrease
    };

    // 룬에서 2차 버프 활성화.
    public void AddBuffType()
    {
        BuffType[] newBuffTypes = new BuffType[]
        {
            BuffType.BulkUp,
            BuffType.EagleEye,
            BuffType.ExtremeSpeed,
            BuffType.IronBody,
            BuffType.Raging
        };

        foreach (BuffType buff in newBuffTypes)
        {
            if (!buffTypes.Contains(buff))
            {
                buffTypes.Add(buff);
            }
        }
    }

    // 룬에서 2차 버프 비활성화.
    public void RemoveBuffType()
    {
        BuffType[] removeBuffTypes = new BuffType[]
        {
            BuffType.BulkUp,
            BuffType.EagleEye,
            BuffType.ExtremeSpeed,
            BuffType.IronBody,
            BuffType.Raging
        };

        foreach (BuffType buff in removeBuffTypes)
        {
            if (buffTypes.Contains(buff))
            {
                buffTypes.Remove(buff);
            }
        }
    }

    // 가능한 버프 확인.
    public BuffType GetRandomBuffType()
    {
        int index = Random.Range(0, buffTypes.Count);
        return buffTypes[index];
    }

    // 룬을 생성.
    public void TrySpawnRune(Vector3 spawnPosition)
    {
        if (runePrefab != null && Random.value <= spawnChance)
        {
            Instantiate(runePrefab, spawnPosition, Quaternion.identity);
        }
    }

    // 룬 생성 확률 변경
    public void RuneSpawnChanceUp(float Chance)
    {
        spawnChance += Chance;
    }
}
