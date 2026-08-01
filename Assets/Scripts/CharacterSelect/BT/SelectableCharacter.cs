using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class SelectableCharacter : MonoBehaviour
{
    [Header("캐릭터 설정")]
    public CharacterType characterType;


    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 매니저가 선택 모드일 때 마우스 클릭 시 선택 이벤트 발생
        if (CharacterSelectManager.Instance != null && CharacterSelectManager.Instance.IsSelectionMode)
        {
            CharacterSelectManager.Instance.SelectCharacter(characterType, this.transform);
        }
    }
}