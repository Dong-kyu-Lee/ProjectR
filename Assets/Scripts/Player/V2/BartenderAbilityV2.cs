using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 바텐더 캐릭터의 고유 능력 구현체
public class BartenderAbilityV2 : MonoBehaviour, IAbilityV2
{
    private BuffManager enemyBuffManager;
    private BuffManager playerBuffManager;
    private Queue<string> bartenderBottles = new Queue<string>();
    private string[] bottleElements = { "Poison", "Burn", "Freeze" };

    public string[] BottleElements => bottleElements;

    public UnityEvent onAbilityUpdated = new UnityEvent();

    void Awake()
    {
        playerBuffManager = GetComponent<BuffManager>();
    }

    // 능력 발동 처리 (Q키는 컨트롤러에서 처리하고 이 메서드를 호출함)
    public void Activate()
    {
        if (!CalcDamage.Instance.IsOnCooldown("BartenderAbility"))
        {
            bartenderBottles.Clear();

            int count = AbilityManager.Instance.bartenderAbility[4] ? 20 : 10;
            AddBartenderBottle(count);

            float cooldown = AbilityManager.Instance.bartenderAbility[5] ? 12f : 20f;
            StartCoroutine(CalcDamage.Instance.Cooldown("BartenderAbility", cooldown));

            onAbilityUpdated?.Invoke();
        }
    }

    public void AddBartenderBottle(int count)
    {
        for (int i = 0; i < count; i++)
        {
            string randomBottle = bottleElements[Random.Range(0, bottleElements.Length)];
            bartenderBottles.Enqueue(randomBottle);;
            // Debug.Log("병 추가 : " + randomBottle);
        }
        if (AbilityManager.Instance.bartenderAbility[4]) BartenderGainBuff();
        onAbilityUpdated.Invoke();
    }

    public string UseBartenderBottle()
    {
        if (bartenderBottles.Count > 0)
        {
            string bottle = bartenderBottles.Dequeue();
            // Debug.Log("사용한 병 : " + bottle);
            onAbilityUpdated.Invoke();
            return bottle;
        }
        return "";
    }

    public void CheckBartenderAbility(GameObject enemy, string bottle)
    {
        enemyBuffManager = enemy.GetComponent<BuffManager>();
        switch (bottle)
        {
            case "Poison":
                enemyBuffManager.ActivateDeBuff(BuffType.Poison, 10f);
                break;
            case "Burn":
                enemyBuffManager.ActivateDeBuff(BuffType.Burn, 6f);
                break;
            case "Freeze":
                enemyBuffManager.ActivateDeBuff(BuffType.Freeze, 8f);
                break;
        }
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

    // 가능한 버프 확인.
    public BuffType GetRandomBuffType()
    {
        int index = Random.Range(0, buffTypes.Count);
        return buffTypes[index];
    }

    // 바텐더 해방 효과 버프 획득
    private void BartenderGainBuff()
    {
        BuffType buffType = GetRandomBuffType();

        playerBuffManager.ActivateBuff(buffType, 30f);
    }

    public void BartenderAttackDebuff(GameObject enemy)
    {
        enemyBuffManager = enemy.GetComponent<BuffManager>();
        enemyBuffManager.ActivateDeBuff(BuffType.Buzzed, 10f);
    }

    public int GetBottleCounts()
    {
        return bartenderBottles.Count;
    }

    public string GetFrontBottleName()
    {
        if (bartenderBottles.Count == 0)
        {
            return "-";
        }
        else
        {
            return bartenderBottles.Peek();
        }
    }

}
