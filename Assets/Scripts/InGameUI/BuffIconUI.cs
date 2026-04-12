using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public struct BuffSpriteEntry
{
    public BuffType buffType;
    public Sprite sprite;
}

public class BuffIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("이미지 컴포넌트")]
    [SerializeField] 
    private Image iconImage;
    [SerializeField] 
    private Image cooldownOverlay;
    [SerializeField]
    private Image baseImage;
    [SerializeField]
    private TextMeshProUGUI buffStackText;

    [Header("버프 종류별 스프라이트")]
    [SerializeField] 
    private List<BuffSpriteEntry> buffSpriteEntries = new List<BuffSpriteEntry>();

    private Dictionary<BuffType, Sprite> buffSpriteDictionary = new Dictionary<BuffType, Sprite>();
    private Buff buffData;
    private BuffToolTipUI tooltip;

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

    private void OnDisable()
    {
        if (tooltip.gameObject.activeSelf)
            tooltip.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateUI();
    }

    public void Initialize(Buff newBuffData)
    {
        buffData = newBuffData;
        Debug.Log($"[BuffIconUI] Initialize 호출됨: {buffData.BuffType}");
        SetSprites();

        iconImage.fillAmount = 1f;
        buffData.InitialDuration = buffData.CurrentDuration;

        if (!buffData.IsDebuff) baseImage.color = Color.green;
        else baseImage.color = Color.red;

        //포션용
        if (buffData.BuffType.ToString().StartsWith("Potion_"))
        {
            iconImage.color = new Color(1f, 1f, 1f, 0.6f); // 반투명 표시
            cooldownOverlay.color = new Color(1f, 1f, 1f, 0.6f);
        }
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
        /*
        if (cooldownOverlay != null)
        {
            cooldownOverlay.sprite = sprite;
        }*/
    }
    private void UpdateUI()
    {
        if (buffData == null) return;

        float fill = Mathf.Clamp01(1f - (buffData.CurrentDuration / buffData.InitialDuration));
        if (buffData.CurrentBuffLevel == 0) buffStackText.text = "";
        else buffStackText.text = (buffData.CurrentBuffLevel + 1).ToString();

        cooldownOverlay.fillAmount = fill;
    }

    // 마우스를 올렸을 때 툴팁 확인
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            if (!tooltip.gameObject.activeSelf)
                tooltip.gameObject.SetActive(true);
            ShowBuffDetail();// 디버깅용 코드 -> 추후 제거 예정
            tooltip.ShowTooltip(buffData, iconImage.sprite);
        }
    }

    // 마우스를 뗐을 때 실행
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip.gameObject.activeSelf)
            tooltip.gameObject.SetActive(false);
    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            if (!tooltip.gameObject.activeSelf)
                tooltip.gameObject.SetActive(true);
            ShowBuffDetail();// 디버깅용 코드 -> 추후 제거 예정
            tooltip.ShowTooltip(buffData.BuffType, iconImage.sprite);
        }
    }*/

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
