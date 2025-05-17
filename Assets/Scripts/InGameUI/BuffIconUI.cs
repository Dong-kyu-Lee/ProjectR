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
    [Header("이미지 컴포넌트")]
    [SerializeField] 
    private Image iconImage;
    [SerializeField] 
    private Image cooldownOverlay;

    [Header("버프 종류별 스프라이트")]
    [SerializeField] 
    private List<BuffSpriteEntry> buffSpriteEntries = new List<BuffSpriteEntry>();

    private Dictionary<BuffType, Sprite> buffSpriteDictionary = new Dictionary<BuffType, Sprite>();
    private Buff buffData;
    private BuffToolTipUI tooltip;

    private float buffStartTime;
    private float buffDuration;

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

    private void Update()
    {
        UpdateUI();
    }

    public void Initialize(Buff newBuffData)
    {
        buffData = newBuffData;
        SetSprites();

        buffStartTime = Time.time;
        buffDuration = buffData.MaxDuration;

        iconImage.fillAmount = 1f;
    }

    public void UpdateData(Buff newBuffData)
    {
        buffData = newBuffData;
        UpdateUI();
    }
    private void SetSprites()
    {
        if (buffData == null || !buffSpriteDictionary.ContainsKey(buffData.BuffType)) return;

        Sprite sprite = buffSpriteDictionary[buffData.BuffType];

        if (iconImage != null)
        {
            iconImage.sprite = sprite;
        }

        if (cooldownOverlay != null)
        {
            cooldownOverlay.sprite = sprite;
        }
    }
    private void UpdateUI()
    {
        if (buffData == null || buffDuration <= 0f) return;

        float elapsed = Time.time - buffStartTime;
        float fill = Mathf.Clamp01(1f - (elapsed / buffDuration));

        iconImage.fillAmount = fill;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            if (!tooltip.gameObject.activeSelf)
                tooltip.gameObject.SetActive(true);
            ShowBuffDetail();// 디버깅용 코드 -> 추후 제거 예정
            tooltip.ShowTooltip(buffData.BuffType, iconImage.sprite);
        }
    }

    public void SetTooltipReference(BuffToolTipUI tooltipUI)
    {
        tooltip = tooltipUI;
    }

    private void ShowBuffDetail() // 디버깅용 코드 -> 추후 제거 예정
    {
        if (buffData == null) return;

        Debug.Log("버프 상세 정보: " + buffData.GetType().Name +
                  "\n지속시간: " + buffData.CurrentDuration + " / " + buffData.MaxDuration +
                  "\n레벨: " + (buffData.CurrentBuffLevel + 1));
    }
}
