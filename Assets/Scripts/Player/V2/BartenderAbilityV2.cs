using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 바텐더 캐릭터의 고유 능력 구현체
public class BartenderAbilityV2 : MonoBehaviour, IAbilityV2
{
    private BuffManager enemyBuffManager;
    private Queue<string> bartenderBottles = new Queue<string>();
    private string[] bottleElements = { "Poison", "Burn", "Freeze" };

    public string[] BottleElements => bottleElements;

    public UnityEvent onAbilityUpdated = new UnityEvent();

    // 능력 발동 처리 (Q키는 컨트롤러에서 처리하고 이 메서드를 호출함)
    public void Activate()
    {
        if (!CalcDamage.Instance.IsOnCooldown("BartenderAbility"))
        {
            bartenderBottles.Clear();

            int count = AbilityManager.Instance.bartenderAbility5 ? 20 : 10;
            AddBartenderBottle(count);

            float cooldown = AbilityManager.Instance.bartenderAbility6 ? 12f : 20f;
            StartCoroutine(CalcDamage.Instance.Cooldown("BartenderAbility", cooldown));

            onAbilityUpdated?.Invoke();
        }
    }

    public void AddBartenderBottle(int count)
    {
        for (int i = 0; i < count; i++)
        {
            string randomBottle = bottleElements[Random.Range(0, bottleElements.Length)];
            bartenderBottles.Enqueue(randomBottle);
            // Debug.Log("병 추가 : " + randomBottle);
        }

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
                enemyBuffManager.ActivateBuff(BuffType.Poison, 10.0f);
                break;
            case "Burn":
                enemyBuffManager.ActivateBuff(BuffType.Burn, 5.0f);
                break;
            case "Freeze":
                enemyBuffManager.ActivateBuff(BuffType.Freeze, 8.0f);
                break;
        }
    }

    public void BartenderAttackDebuff(GameObject enemy)
    {
        enemyBuffManager = enemy.GetComponent<BuffManager>();
        enemyBuffManager.ActivateBuff(BuffType.Buzzed, 10.0f);
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
