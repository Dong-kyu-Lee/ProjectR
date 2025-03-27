using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class LiberationDesc : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text abilityText;
    [SerializeField] private string abilityDesc;

    // 마우스가 들어오면 툴팁 표시.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (abilityText != null)
        {
            abilityText.text = abilityDesc;
        }
    }

    // 마우스가 나가면 툴팁 숨김.
    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
