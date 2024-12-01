using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatusEffectTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject effectTooltip;
    [SerializeField] private StatusTooltip statusTooltip;
    public Image image;
    public Color defaultColor;

    // 시작 시 툴팁 숨기기.
    void Start()
    {
        defaultColor = Color.white;
        image = gameObject.GetComponent<Image>();

        if (effectTooltip != null)
        {
            effectTooltip.SetActive(false);
        }
    }

    // 마우스가 들어오면 툴팁 표시.
    public void OnPointerEnter(PointerEventData eventData)
    {
        string name = gameObject.GetComponent<StatusEffectInformation>().statusEffectName;
        string desc = gameObject.GetComponent<StatusEffectInformation>().statusEffectDesc;

        image.color = Color.grey;

        if (effectTooltip != null)
        {
            statusTooltip.SetupTooltip(name, desc);
            effectTooltip.SetActive(true);
        }
    }

    // 마우스가 나가면 툴팁 숨김.
    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = defaultColor;
        effectTooltip.SetActive(false);
    }
}
