using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    private static CharacterSelectUI instance;
    public static CharacterSelectUI Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("CharacterSelectUI");
                instance = gameObject.AddComponent<CharacterSelectUI>();
            }
            return instance;
        }
    }
    public TextMeshProUGUI uiText;

    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        if (uiText  == null)
        {
            uiText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    // 캐릭터 선택 UI 텍스트 설정
    public void SetText(string text)
    {
        if(uiText.gameObject.activeInHierarchy == false)
        {
            uiText.gameObject.SetActive(true);
        }

        uiText.text = text;
    }

    // 캐릭터 선택 UI 텍스트 숨김
    public void HideText()
    {
        if (uiText.gameObject.activeInHierarchy == true)
        {
            uiText.gameObject.SetActive(false);
        }
    }
}
