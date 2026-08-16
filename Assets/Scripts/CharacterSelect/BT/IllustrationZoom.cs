using UnityEngine;
using UnityEngine.EventSystems;

// IScrollHandler: 마우스 휠 스크롤 감지
// IDragHandler: 마우스 클릭 후 드래그 감지
public class IllustrationZoom : MonoBehaviour, IScrollHandler, IDragHandler
{
    [Header("확대/축소 설정")]
    [SerializeField] private float zoomSpeed = 0.1f; // 휠 한 번에 확대되는 속도
    [SerializeField] private float minZoom = 1f;   // 최소 크기 (원본)
    [SerializeField] private float maxZoom = 4f;   // 최대 4배까지 확대 가능

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        // 팝업 창이 새로 열릴 때마다 크기와 위치를 깔끔하게 원상복구
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    // 마우스 휠을 굴릴 때 실행됨
    public void OnScroll(PointerEventData eventData)
    {
        float scroll = eventData.scrollDelta.y;
        Vector3 newScale = rectTransform.localScale + (Vector3.one * scroll * zoomSpeed);

        // 지정한 최소(1)~최대(4) 배율 사이를 벗어나지 못하게 고정
        newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);
        newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);
        newScale.z = Mathf.Clamp(newScale.z, minZoom, maxZoom);

        rectTransform.localScale = newScale;
    }

    // 마우스를 클릭한 채로 끌 때 실행됨
    public void OnDrag(PointerEventData eventData)
    {
        // 원본보다 커진 상태(확대된 상태)에서만 이미지를 드래그해서 볼 수 있도록 허용
        if (rectTransform.localScale.x > minZoom)
        {
            if (parentCanvas != null)
            {
                // 해상도 비율(scaleFactor)에 맞춰 마우스 이동 거리만큼 이미지를 이동
                rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
            }
            else
            {
                rectTransform.anchoredPosition += eventData.delta;
            }
        }
    }
}