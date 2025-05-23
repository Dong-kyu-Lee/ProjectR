using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BartenderAbility : MonoBehaviour
{
    private BuffManager enemyBuffManager;
    private Queue<string> Bartender_Bottles = new Queue<string>();
    private string[] bottleElements = { "Poison", "Burn", "Freeze" };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !CalcDamage.Instance.IsOnCooldown("BartenderAbility"))
        {
            if (AbilityManager.Instance.bartenderAbility5) AddBartenderBottle(20);
            else AddBartenderBottle(10);
            if (AbilityManager.Instance.bartenderAbility6) StartCoroutine(CalcDamage.Instance.Cooldown("BartenderAbility", 12f));
            else StartCoroutine(CalcDamage.Instance.Cooldown("BartenderAbility", 20f));
        }
    }

    public void BartenderAttackDebuff(GameObject enemy)
    {
        enemyBuffManager = enemy.GetComponent<BuffManager>();
        enemyBuffManager.ActivateBuff(BuffType.Buzzed, 10.0f);
    }

    public void AddBartenderBottle(int count)
    {
        for (int i = 0; i < count; i++)
        {
            string randomBottle = bottleElements[Random.Range(0, bottleElements.Length)];
            Bartender_Bottles.Enqueue(randomBottle);
            Debug.Log("병 추가 : " + randomBottle);
        }
    }

    public string UseBartenderBottle()
    {
        if (Bartender_Bottles.Count > 0)
        {
            string bottle = Bartender_Bottles.Dequeue();
            Debug.Log("사용한 병 : " + bottle);
            return bottle;
        }
        return "";
    }

    public void CheckBartenderAbility(string bottle)
    {
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
            case "":
                break;
        }
    }
}
