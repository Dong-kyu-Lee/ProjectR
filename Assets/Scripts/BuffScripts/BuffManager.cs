using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    // 활성화된 버프들을 BuffType을 키로 하여 저장.
    private Dictionary<BuffType, Buff> activeBuffs = new Dictionary<BuffType, Buff>();
    public Dictionary<BuffType, Buff> ActiveBuffDict => activeBuffs;

    public Buff GenerateBuff(BuffType type, float duration, GameObject target)
    {
        return BuffFactory.Instance.CreateBuff(type, duration, target);
    }

    public void ActivateBuff(BuffType type, float duration)
    {
        if (activeBuffs.ContainsKey(type))
        {
            activeBuffs[type].BuffOverlap(duration);
            Debug.Log("지속시간 갱신");
        }
        else
        {
            Buff newBuff = GenerateBuff(type, duration, gameObject);
            activeBuffs.Add(type, newBuff);
            newBuff.ApplyBuffEffect();
            StartCoroutine(StartBuffEffect(newBuff));
            Debug.Log($"버프[{type}]적용");
        }
    }

    //활성중인 버프 레벨을 1단계 올리는 메서드
    public void ActiveBuffLevelUpOnce(BuffType type)
    {
        if (activeBuffs.ContainsKey(type))
        {
            activeBuffs[type].BuffOverlap(0.0f);
        }
    }

    /// 외부에서 버프를 강제 제거할 때 사용하는 메서드
    public void DeactivateBuff(BuffType type)
    {
        if (activeBuffs.TryGetValue(type, out Buff buff))
        {
            buff.RemoveBuffEffect();
            activeBuffs.Remove(type);
            Debug.Log($"버프 [{type}] 강제 제거됨.");
        }
        else
        {
            Debug.LogWarning($"DeactivateBuff: 버프 [{type}]가 활성화되어 있지 않습니다.");
        }
    }
    private IEnumerator StartBuffEffect(Buff buff)
    {
        // 매 1초마다 버프 효과를 적용하고 지속시간을 감소시킵니다.
        while (buff.CurrentDuration > 0)
        {
            buff.DoActionOnActivate(1.0f);
            yield return new WaitForSeconds(1.0f);
        }
        buff.RemoveBuffEffect();

        BuffType type = GetBuffTypeFromBuff(buff);
        if (activeBuffs.ContainsKey(type))
        {
            activeBuffs.Remove(type);
        }
        Debug.Log($"버프 [{type}] 지속시간 만료되어 제거됨.");
    }

    private BuffType GetBuffTypeFromBuff(Buff buff)
    {
        return buff.BuffType;
    }

    public List<Buff> GetActiveBuffs()
    {
        return new List<Buff>(activeBuffs.Values);
    }

    public void ClearPotionBuffs()
    {
        List<BuffType> potionBuffsToRemove = new List<BuffType>();

        foreach (var buffType in activeBuffs.Keys)
        {
            if (buffType.ToString().StartsWith("Potion_"))
            {
                potionBuffsToRemove.Add(buffType);
            }
        }

        foreach (var buffType in potionBuffsToRemove)
        {
            DeactivateBuff(buffType);
            Debug.Log($"[BuffManager] 스테이지 전환으로 포션 버프 제거됨: {buffType}");
        }
    }

}