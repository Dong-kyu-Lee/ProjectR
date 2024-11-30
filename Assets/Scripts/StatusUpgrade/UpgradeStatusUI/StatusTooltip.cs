using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusTooltip : MonoBehaviour
{
    [SerializeField] private Text statusEffectNameText;
    [SerializeField] private Text statusEffectDescText;

    private float halfwidth;
    private RectTransform rectTransform;

    private void Start()
    {
        halfwidth = GetComponentInParent<CanvasScaler>().referenceResolution.x * 0.5f;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // 툴팁이 마우스를 따라가는 기능.
        transform.position = Input.mousePosition;

        // 툴팁 피벗 조정.
        if (rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x > halfwidth)
            rectTransform.pivot = new Vector2(1, 1);
        else
            rectTransform.pivot = new Vector2(0, 1);
    }

    // 툴팁 정보 설정.
    public void SetupTooltip(string name, string desc)
    {
         statusEffectNameText.text = name;
         statusEffectDescText.text = desc;
    }
}
