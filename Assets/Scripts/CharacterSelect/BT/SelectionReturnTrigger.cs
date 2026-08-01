using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SelectionReturnTrigger : MonoBehaviour
{
    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (CharacterSelectManager.Instance != null && !CharacterSelectManager.Instance.IsSelectionMode)
            {
                CharacterSelectManager.Instance.EnterSelectionMode();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (CharacterSelectUI.Instance != null) CharacterSelectUI.Instance.SetText("'E' 키를 눌러 캐릭터 변경");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (CharacterSelectUI.Instance != null) CharacterSelectUI.Instance.HideText();
        }
    }
}
