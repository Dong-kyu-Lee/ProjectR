using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
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
            Debug.Log($"버프 [{type}] 이미 존재하므로 지속시간 갱신: {existingBuff.CurrentDuration}");
        }
        else
        {
            Buff newBuff = GenerateBuff(type, duration, gameObject);
            activeBuffs.Add(type, newBuff);
            StartCoroutine(StartBuffEffect(newBuff));
        }
    }

    private IEnumerator StartBuffEffect(Buff buff)
    {
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
    }

    private BuffType GetBuffTypeFromBuff(Buff buff)
    {
        return (BuffType)System.Enum.Parse(typeof(BuffType), buff.GetType().Name);
    }

    public List<Buff> GetActiveBuffs()
    {
        return new List<Buff>(activeBuffs.Values);
    }

}