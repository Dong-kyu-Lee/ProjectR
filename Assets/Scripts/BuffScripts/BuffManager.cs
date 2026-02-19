using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    // 활성화된 버프들을 BuffType을 키로 하여 저장.
    public Dictionary<BuffType, Buff> ActiveBuffDict = new Dictionary<BuffType, Buff>();

    private Collider col;

    private void Awake()
    {
        col = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (ActiveBuffDict.Count == 0) return;

        float currentTime = Time.time;
        var keys = ActiveBuffDict.Keys.ToList();

        foreach (var type in keys)
        {
            Buff buff = ActiveBuffDict[type];

            // 지속시간 감소
            buff.CurrentDuration -= Time.deltaTime;

            // 종료 체크
            if (buff.CurrentDuration <= 0)
            {
                buff.RemoveBuffEffect();
                ActiveBuffDict.Remove(type);
                Debug.Log($"버프 [{type}] 만료.");
                return;
            }

            // 틱 주기 계산
            float tickInterval = buff.BuffEffectTick;
            if (buff.TargetObject() == "Enemy" && CalcDamage.Instance.curseEffect16 &&
               (type == BuffType.Burn || type == BuffType.Poison))
            {
                tickInterval /= 2f;
            }

            // 틱 실행
            if (Time.time >= buff.LastTickTime + tickInterval)
            {
                buff.DoActionOnActivate(tickInterval);
                buff.LastTickTime += tickInterval;
            }
        }
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

        if (ActiveBuffDict.ContainsKey(type))
        {
            ActiveBuffDict[type].BuffOverlap(duration);
            Debug.Log($"[{type}] 지속시간 갱신 {ActiveBuffDict[type].CurrentDuration}");
            Debug.Log($"최대 지속시간 {ActiveBuffDict[type].MaxDuration}");
        }
        else
        {
            Buff newBuff = GenerateBuff(type, duration, gameObject);
            ActiveBuffDict.Add(type, newBuff);
            newBuff.ApplyBuffEffect();
            StartBuffEffect(newBuff);
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
        if (ActiveBuffDict.ContainsKey(type))
        {
            ActiveBuffDict[type].BuffOverlap(duration);
            Debug.Log("지속시간 갱신");
        }
        else
        {
            Buff newBuff = GenerateBuff(type, duration, gameObject);
            ActiveBuffDict.Add(type, newBuff);
            newBuff.ApplyBuffEffect();
            StartBuffEffect(newBuff);
            Debug.Log($"버프[{type}]적용");
        }
    }

    //활성중인 버프 레벨을 1단계 올리는 메서드
    public void ActiveBuffLevelUpOnce(BuffType type)
    {
        if (ActiveBuffDict.ContainsKey(type))
        {
            ActiveBuffDict[type].BuffOverlap(0.0f);
        }
    }

    /// 외부에서 버프를 강제 제거할 때 사용하는 메서드
    public void DeactivateBuff(BuffType type)
    {
        if (ActiveBuffDict.TryGetValue(type, out Buff buff))
        {
            buff.RemoveBuffEffect();
            ActiveBuffDict.Remove(type);
            Debug.Log($"버프 [{type}] 강제 제거됨.");
        }
        else
        {
            Debug.LogWarning($"DeactivateBuff: 버프 [{type}]가 활성화되어 있지 않습니다.");
        }
    }

    private void StartBuffEffect(Buff buff)
    {
        BuffType type = GetBuffTypeFromBuff(buff);
        buff.LastTickTime = Time.time; // 현재 시간으로 초기화
        ActiveBuffDict[type] = buff;
    }

    private BuffType GetBuffTypeFromBuff(Buff buff)
    {
        return buff.BuffType;
    }

    public List<Buff> GetActiveBuffs()
    {
        return new List<Buff>(ActiveBuffDict.Values);
    }

    public void ClearPotionBuffs()
    {
        List<BuffType> potionBuffsToRemove = new List<BuffType>();

        foreach (var buffType in ActiveBuffDict.Keys)
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