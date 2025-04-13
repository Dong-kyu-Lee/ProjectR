using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    private static BuffManager instance;

    public static BuffManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject buffManager = new GameObject("BuffManager");
                instance = buffManager.AddComponent<BuffManager>();
                DontDestroyOnLoad(buffManager);
            }
            return instance;
        }
    }
    // 활성화된 버프들을 BuffType을 키로 하여 저장.
    private Dictionary<BuffType, Buff> activeBuffs = new Dictionary<BuffType, Buff>();

    public Buff GenerateBuff(BuffType type, float duration, GameObject target)
    {
        return BuffFactory.Instance.CreateBuff(type, duration, target);
    }

    public void ActivateBuff(BuffType type, float duration)
    {
        if (activeBuffs.ContainsKey(type))
        {
            // 이미 같은 타입의 버프가 존재하면 지속시간을 갱신
            Buff existingBuff = activeBuffs[type];
            existingBuff.CurrentDuration = existingBuff.MaxDuration;
            Debug.Log("지속시간 갱신");
        }
        else
        {
            Buff newBuff = GenerateBuff(type, duration, GameManager.Instance.CurrentPlayer);
            activeBuffs.Add(type, newBuff);
            StartCoroutine(StartBuffEffect(newBuff));
            Debug.Log($"버프[{type}]적용");
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
}