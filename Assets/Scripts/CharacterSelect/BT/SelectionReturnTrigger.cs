using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SelectionReturnTrigger : MonoBehaviour
{
    [Header("이 구역의 주인")]
    public CharacterType ownerCharacterType;

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
            // 현재 조작 중인 플레이어의 타입이 이 구역의 주인과 일치할 때만 작동
            if (PlayerManager.Instance.CurrentCharacterType == ownerCharacterType)
            {
                isPlayerNear = true;
                if (CharacterSelectUI.Instance != null) CharacterSelectUI.Instance.SetText("'E' 키를 눌러 캐릭터 변경");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 나갈 때도 타입이 일치할 때만 UI 텍스트를 숨김 처리
            if (PlayerManager.Instance.CurrentCharacterType == ownerCharacterType)
            {
                isPlayerNear = false;
                if (CharacterSelectUI.Instance != null) CharacterSelectUI.Instance.HideText();
            }
        }
    }
}