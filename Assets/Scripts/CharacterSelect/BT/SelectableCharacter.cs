using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class SelectableCharacter : MonoBehaviour
{
    [Header("캐릭터 설정")]
    public CharacterType characterType;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // [신규 추가] 커질 때 바닥을 뚫지 않도록 위로 올려줄 수치 (인스펙터에서 조절 가능)
    [Header("하이라이트 효과 설정")]
    [SerializeField] private float hoverScaleMultiplier = 1.2f;
    [SerializeField] private float hoverYOffset = 0.15f;

    private Vector3 originalScale;
    private Vector3 originalPosition; // [신규 추가] 원래 위치 기억

    private void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 시작할 때 원래 크기와 위치를 저장해둠
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (CharacterSelectManager.Instance != null && CharacterSelectManager.Instance.IsSelectionMode)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(0.9f, 0.9f, 0.9f);

                // 크기를 키우고, 동시에 설정한 수치만큼 Y축으로 위로 들어올림
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

            // 크기와 위치 모두 원래대로 완벽하게 복구
            transform.localScale = originalScale;
            transform.localPosition = originalPosition;
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (CharacterSelectManager.Instance != null && CharacterSelectManager.Instance.IsSelectionMode)
        {
            OnMouseExit();
            CharacterSelectManager.Instance.SelectCharacter(characterType, this.transform);
        }
    }
}