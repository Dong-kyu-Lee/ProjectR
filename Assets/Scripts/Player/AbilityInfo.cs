using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityInfo : MonoBehaviour
{
    [SerializeField]
    BlacksmithAbility blacksmithAbility;

    [SerializeField]
    Text activateText;

    [SerializeField]
    Button enchantButton;

    [SerializeField]
    Button destroyButton;

    void Start()
    {
        blacksmithAbility.onAbilityUpdated.AddListener(UpdateAbilityInfo);
        UpdateAbilityInfo();
    }

    void Update()
    {
        
    }

    public void UpdateAbilityInfo()
    {
        if (blacksmithAbility.IsActivated)
        {
            activateText.text = "활성화 상태 : on";
            enchantButton.gameObject.SetActive(true);
            destroyButton.gameObject.SetActive(true);
        }
        else
        {
            activateText.text = "활성화 상태 : off";
            enchantButton.gameObject.SetActive(false);
            destroyButton.gameObject.SetActive(false);
        }
    }
}
