using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class SelectableCharacter : MonoBehaviour
{
    [Header("캐릭터 설정")]
    public CharacterType characterType;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("하이라이트 효과 설정")]
    [SerializeField] private float hoverScaleMultiplier = 1.2f;
    [SerializeField] private float hoverYOffset = 0.15f;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        //  상세 정보 창을 보는 중(IsViewingDetails == true)이 아닐 때만 커짐
        if (CharacterSelectManager.Instance != null &&
            CharacterSelectManager.Instance.IsSelectionMode &&
            !CharacterSelectManager.Instance.IsViewingDetails)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(0.9f, 0.9f, 0.9f);
                transform.localScale = originalScale * hoverScaleMultiplier;
                transform.localPosition = originalPosition + new Vector3(0, hoverYOffset, 0);
            }
        }
    }

    private void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            transform.localScale = originalScale;
            transform.localPosition = originalPosition;
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // 상세 정보 창을 보는 중에는 뒤에 있는 캐릭터가 다시 클릭되지 않도록 방어
        if (CharacterSelectManager.Instance != null &&
            CharacterSelectManager.Instance.IsSelectionMode &&
            !CharacterSelectManager.Instance.IsViewingDetails)
        {
            OnMouseExit();
            CharacterSelectManager.Instance.SelectCharacter(characterType, this.transform);
        }
    }
}