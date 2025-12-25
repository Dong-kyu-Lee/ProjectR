using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    // 활성화된 버프들을 BuffType을 키로 하여 저장.
    private Dictionary<BuffType, Buff> activeBuffs = new Dictionary<BuffType, Buff>();
    public Dictionary<BuffType, Buff> ActiveBuffDict => activeBuffs;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<CapsuleCollider>();
    }

    public Buff GenerateBuff(BuffType type, float duration, GameObject target)
    {
        return BuffFactory.Instance.CreateBuff(type, duration, target);
    }

    // 버프 활성화
    public void ActivateBuff(BuffType type, float duration)
    {
        if (gameObject.CompareTag("Player"))
        {
            duration *= (1 + CalcDamage.Instance.additionalBuffTime);
        }

        if (activeBuffs.ContainsKey(type))
        {
            activeBuffs[type].BuffOverlap(duration);
            Debug.Log($"지속시간 갱신 {activeBuffs[type].CurrentDuration}");
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

    // 디버프 활성화
    public void ActivateDeBuff(BuffType type, float duration)
    {
        if (gameObject.CompareTag("Enemy"))
        {
            duration *= (1 + CalcDamage.Instance.additionalDebuffTime);
        }
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
        BuffType type = GetBuffTypeFromBuff(buff);
        // 매 2초마다 버프 효과를 적용하고 지속시간을 감소시킵니다.
        while (buff.CurrentDuration > 0)
        {
            if (buff.TargetObject() == "Enemy" && CalcDamage.Instance.curseEffect16 && (type == BuffType.Burn || type == BuffType.Poison))
            {
                buff.DoActionOnActivate(buff.BuffEffectTick / 2);
                BuffEffectManager.Instance.PlayBuffEffect(type, transform.position, col);
                yield return new WaitForSeconds(buff.BuffEffectTick / 2);
            }
            else
            {
                buff.DoActionOnActivate(buff.BuffEffectTick);
                BuffEffectManager.Instance.PlayBuffEffect(type, transform.position, col);
                yield return new WaitForSeconds(buff.BuffEffectTick);
            }
        }
        buff.RemoveBuffEffect();

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