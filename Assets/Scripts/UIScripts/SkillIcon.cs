using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SkillToolTipUI tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            // 디버깅용 메서드(ShowBuffDetail) 호출 제거하여 깔끔하게 정리
            tooltip.ShowTooltip(GameManager.Instance.CurrentPlayer.GetComponent<PlayerControllerBase>().playerName);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.Hide();
        }
    }
}
