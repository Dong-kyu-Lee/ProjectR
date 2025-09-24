using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class LiberationDesc : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private LiberationSystem liberationSystem;
    [SerializeField] private Text abilityText;
    [SerializeField] private Text abilityPriceText;
    [SerializeField] private int abilityIndex;
    [SerializeField] private string abilityDesc;
    public int abilityPrice;
    public Image image;
    public Color defaultColor;

    void Awake()
    {
        defaultColor = new Color(100f / 255f, 100f / 255f, 100f / 255f);
        image = GetComponent<Image>();
        liberationSystem = transform.parent.GetComponent<LiberationSystem>();
    }

    // 마우스가 들어오면 설명 표시.
    public void OnPointerEnter(PointerEventData eventData)
    {
        liberationSystem.currentAbility = abilityIndex;
        image.color = Color.grey;
        if (abilityText != null) abilityText.text = abilityDesc;
        if (abilityPriceText != null) abilityPriceText.text = abilityPrice.ToString() + " 필요";
    }

    // 마우스가 나가면 설명 숨김.
    public void OnPointerExit(PointerEventData eventData)
    {
        ResetDescState();
    }

    // 설명 초기화.
    private void ResetDescState()
    {
        liberationSystem.currentAbility = -1;
        image.color = defaultColor;
        if (abilityText != null) abilityText.text = "";
        if (abilityPriceText != null) abilityPriceText.text = "";
    }

    private void OnDisable()
    {
        ResetDescState();
    }
}
