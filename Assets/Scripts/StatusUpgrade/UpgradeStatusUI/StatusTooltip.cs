using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusTooltip : MonoBehaviour
{
    [SerializeField] private Text statusEffectNameText;
    [SerializeField] private Text statusEffectDescText;

    // 툴팁이 마우스를 따라가는 기능.
    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    // 툴팁 정보 설정.
    public void SetupTooltip(string name, string desc)
    {
         statusEffectNameText.text = name;
         statusEffectDescText.text = desc;
    }
}
