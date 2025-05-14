using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct BuffSpriteEntry
{
    public BuffType buffType;
    public Sprite sprite;
}

public class BuffIconUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] 
    private Image iconImage; //버프 이미지
    [SerializeField] 
    private Image cooldownOverlay;

    
    [SerializeField] private List<BuffSpriteEntry> buffSpriteEntries = new List<BuffSpriteEntry>();
    // 내부적으로 매핑된 딕셔너리
    private Dictionary<BuffType, Sprite> buffSpriteDictionary = new Dictionary<BuffType, Sprite>();

    // 현재 적용된 버프 데이터를 저장
    private Buff buffData;

    private void Awake()
    {
        foreach (BuffSpriteEntry entry in buffSpriteEntries)
        {
            if (!buffSpriteDictionary.ContainsKey(entry.buffType))
            {
                buffSpriteDictionary.Add(entry.buffType, entry.sprite);
            }
        }
    }

    public void Initialize(Buff newBuffData)
    {
        buffData = newBuffData;
        cooldownOverlay.sprite = iconImage.sprite;
        UpdateUI();
    }

    public void UpdateData(Buff newBuffData)
    {
        buffData = newBuffData;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (buffData != null)
        {
            if (cooldownOverlay != null && buffData.MaxDuration > 0f)
            {
                cooldownOverlay.fillAmount = buffData.CurrentDuration / buffData.MaxDuration;
            }

            if (iconImage != null)
            {
                BuffType type = buffData.BuffType;
                if (buffSpriteDictionary.ContainsKey(type))
                {
                    iconImage.sprite = buffSpriteDictionary[type];
                }
            }
        }
    }
    private BuffType GetBuffTypeFromBuffData(Buff buff)
    {
        return buff.BuffType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowBuffDetail();
    }

    private void ShowBuffDetail()
    {
        Debug.Log("버프 상세 정보: " + buffData.GetType().Name +
                  "\n지속시간: " + buffData.CurrentDuration + " / " + buffData.MaxDuration +
                  "\n레벨: " + (buffData.CurrentBuffLevel + 1));
    }
}