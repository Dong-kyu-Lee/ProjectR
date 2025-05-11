using System.Collections.Generic;
using UnityEngine;

public class BuffUIManager : MonoBehaviour
{
    [SerializeField]
    private Transform buffPanel;
    [SerializeField]
    private Transform debuffPanel;
    [SerializeField]
    private GameObject buffIconPrefab;
    private Dictionary<BuffType, BuffInfo> buffInfoMapping;

    void Awake()
    {
        BuffInfo[] infos = Resources.LoadAll<BuffInfo>("BuffInfo");
        buffInfoMapping = new Dictionary<BuffType, BuffInfo>();
        foreach (BuffInfo info in infos)
        {
            if (!buffInfoMapping.ContainsKey(info.buffType))
                buffInfoMapping.Add(info.buffType, info);
        }
    }

    public void UpdateBuffUI(List<Buff> activeBuffs)
    {
        foreach (Transform child in buffPanel)
            Destroy(child.gameObject);
        foreach (Transform child in debuffPanel)
            Destroy(child.gameObject);

        foreach (Buff buff in activeBuffs)
        {
            BuffType type = GetBuffTypeFromBuff(buff);
            if (buffInfoMapping.TryGetValue(type, out BuffInfo info))
            {
                Transform parentPanel = info.isDebuff ? debuffPanel : buffPanel;
                GameObject iconObj = Instantiate(buffIconPrefab, parentPanel);
                BuffIconUI iconUI = iconObj.GetComponent<BuffIconUI>();
                if (iconUI != null)
                    iconUI.Initialize(info, buff);
            }
            else
            {
                Debug.LogWarning("등록된 BuffInfo가 없습니다: " + type);
            }
        }
    }

    private BuffType GetBuffTypeFromBuff(Buff buff)
    {
        return (BuffType)System.Enum.Parse(typeof(BuffType), buff.GetType().Name);
    }
}