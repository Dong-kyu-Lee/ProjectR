using UnityEngine;

// 던전 배경 이미지의 이동을 위한 클래스
// 배경 레이어들의 사이즈는 모두 동일한 것으로 가정한다.
[DefaultExecutionOrder(100)] // CinemachineBrain(LateUpdate) 이후에 실행되어야 떨림이 없음
public class BGImageMove : MonoBehaviour
{
    public static BGImageMove Instance { get; private set; }

    [Header("Background Image Settings")]

    [Tooltip("배경 이미지 레이어 : 인덱스가 클 수록 가까이 있는 것")]
    [SerializeField]
    private Transform[] backgroundImageLayer;

    [Tooltip("레이어별 원근 계수. 0 = 방과 같은 평면, 1 = 무한히 멀어 화면에 고정. backgroundImageLayer와 순서를 맞출 것")]
    [SerializeField]
    private float[] parallaxFactor = { 0.85f, 0.65f, 0.4f, 0.15f };

    [Tooltip("세로 시차를 가로 대비 얼마나 적용할지")]
    [Range(0f, 1f)]
    [SerializeField]
    private float verticalRatio = 0.6f;

    [Tooltip("배경 이미지 중심 좌표(방 원점 기준)")]
    [SerializeField]
    private Vector3 backgroundOffset = new Vector3(19.5f, 19.5f, 0f);

    [Tooltip("배경 이미지 크기")]
    [SerializeField]
    private float backgroundSize = 1.3f;

    [Tooltip("방 이동 시 배경이 따라오는 시간(초). 0이면 즉시 이동")]
    [SerializeField]
    private float followTime = 0.4f;

    private Camera targetCamera;
    private Vector3 anchor;        // 배경이 현재 붙어 있는 방의 중심
    private Vector3 targetAnchor;  // 이동해야 할 방의 중심
    private Vector3 anchorVelocity;
    private bool hasAnchor = false;

    private void Awake()
    {
        Instance = this;
        targetCamera = Camera.main;

        for (int i = 0; i < backgroundImageLayer.Length; ++i)
        {
            if (backgroundImageLayer[i] == null) continue;
            backgroundImageLayer[i].localScale = new Vector3(backgroundSize, backgroundSize, 1f);
            // 인덱스가 클수록(가까울수록) 앞에 그려지도록 정렬 순서 지정
            SpriteRenderer renderer = backgroundImageLayer[i].GetComponent<SpriteRenderer>();
            if (renderer != null) renderer.sortingOrder = i;
        }
        SetVisible(false); // 방에 진입할 때 켜짐
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // 배경 이미지를 지정한 방 뒤로 이동시키는 함수
    // roomPosition : 방의 원점 좌표(RoomInstance.transform.position)
    // useSpriteBackground : 이 방이 배경 타일 대신 배경 이미지를 사용하는지 여부
    // instant : 즉시 이동 여부(던전 생성 직후 등)
    public void MoveToRoom(Vector3 roomPosition, bool useSpriteBackground, bool instant = false)
    {
        SetVisible(useSpriteBackground);
        if (useSpriteBackground == false) { hasAnchor = false; return; }

        Vector3 newAnchor = roomPosition + backgroundOffset;
        // 워프 이동(위/아래 방)은 카메라도 순간이동하므로 배경도 즉시 이동시킨다.
        bool snap = instant || hasAnchor == false || Mathf.Approximately(anchor.y, newAnchor.y) == false;

        targetAnchor = newAnchor;
        if (snap)
        {
            anchor = newAnchor;
            anchorVelocity = Vector3.zero;
        }
        hasAnchor = true;

        ApplyParallax(); // 이전 방 위치가 한 프레임 보이는 것 방지
    }

    private void LateUpdate()
    {
        if (hasAnchor == false) return;

        // 방 이동 중이면 기준점을 목표 방 중심으로 부드럽게 이동
        if (anchor != targetAnchor)
            anchor = Vector3.SmoothDamp(anchor, targetAnchor, ref anchorVelocity, followTime);

        ApplyParallax();
    }

    // 카메라 위치에 따라 각 레이어를 원근감 있게 배치하는 함수
    private void ApplyParallax()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null) return;

        Vector3 delta = targetCamera.transform.position - anchor;

        for (int i = 0; i < backgroundImageLayer.Length; ++i)
        {
            if (backgroundImageLayer[i] == null) continue;

            float k = (i < parallaxFactor.Length) ? parallaxFactor[i] : 0f;
            Vector3 position = anchor;
            position.x += delta.x * k;
            position.y += delta.y * k * verticalRatio;
            position.z = backgroundImageLayer[i].position.z;
            backgroundImageLayer[i].position = position;
        }
    }

    // 배경 이미지 레이어들의 표시 여부를 설정하는 함수
    private void SetVisible(bool visible)
    {
        for (int i = 0; i < backgroundImageLayer.Length; ++i)
        {
            if (backgroundImageLayer[i] != null)
                backgroundImageLayer[i].gameObject.SetActive(visible);
        }
    }
}
