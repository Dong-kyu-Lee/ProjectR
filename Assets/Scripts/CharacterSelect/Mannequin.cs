using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour
{
    bool isPlayerNear = false;
    public CharacterType characterType;
    public CharacterSelect characterSelect;
    public string characterName;

    void Start()
    {
        if(characterSelect == null)
        {
            Debug.LogError("CharacterSelect is null");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isPlayerNear)
        {
            characterSelect.SelectCharacter(characterType, transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNear = true;
            // 'E' 키 활성화
            CharacterSelectUI.Instance.SetText($"'E' 키를 눌러 <color=yellow>{characterName}</color> 로 변경하기");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNear = false;
            // 'E' 키 비활성화
            CharacterSelectUI.Instance.HideText();
        }
    }
}
