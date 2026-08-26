using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video; // 비디오 플레이어 접근용

public class CharacterSelectUI : MonoBehaviour
{
    private static CharacterSelectUI instance;
    public static CharacterSelectUI Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<CharacterSelectUI>();
            return instance;
        }
    }

    [Header("기존 상호작용 텍스트")]
    public TextMeshProUGUI uiText;

    [Header("상세 정보 패널 (좌/우)")]
    [SerializeField] private RectTransform leftPanel;
    [SerializeField] private RectTransform rightPanel;
    [SerializeField] private float slideDuration = 0.3f; // 스르륵 나타나는 시간

    [Header("상단 안내 배너 UI")]
    [SerializeField] private RectTransform topBannerPanel;
    [SerializeField] private float bannerSlideDuration = 0.4f;

    [Header("UI Stack System")]
    [SerializeField] private CharacterSelectEscHandler escHandler;

    [Header("좌측 패널 UI 요소 (스토리)")]
    [SerializeField] private Text characterNameText;
    [SerializeField] private Text backgroundStoryText;
    [SerializeField] private Image characterIllustrationImage;

    [Header("우측 패널 UI 요소 (스킬)")]
    [SerializeField] private Image skillIconImage;
    [SerializeField] private Text skillDescriptionText;
    [SerializeField] private VideoPlayer skillVideoPlayer;

    [Header("확대 비디오 UI")]
    [SerializeField] private GameObject largeVideoPanel;

    [Header("확대 일러스트 UI")]
    [SerializeField] private GameObject largeIllustrationPanel;
    [SerializeField] private Image largeIllustrationImage;

    private Vector2 leftPanelHiddenPos;
    private Vector2 leftPanelShownPos;
    private Vector2 rightPanelHiddenPos;
    private Vector2 rightPanelShownPos;

    private Vector2 topBannerHiddenPos;
    private Vector2 topBannerShownPos;

    private Coroutine panelCoroutine;
    private Coroutine bannerCoroutine;

    void Start()
    {
        if (instance == null) instance = this;
        if (uiText == null) uiText = GetComponentInChildren<TextMeshProUGUI>();

        // 시작할 때 패널들의 현재 위치를 '보여지는 위치'로 저장하고 화면 밖으로 밀어냄
        if (leftPanel != null && rightPanel != null)
        {
            leftPanelShownPos = leftPanel.anchoredPosition;
            leftPanelHiddenPos = new Vector2(leftPanelShownPos.x - 1000f, leftPanelShownPos.y);
            leftPanel.anchoredPosition = leftPanelHiddenPos;

            rightPanelShownPos = rightPanel.anchoredPosition;
            rightPanelHiddenPos = new Vector2(rightPanelShownPos.x + 1000f, rightPanelShownPos.y);
            rightPanel.anchoredPosition = rightPanelHiddenPos;
        }

        // 시작할 때 상단 배너의 위치를 기억하고 화면 위로 숨김
        if (topBannerPanel != null)
        {
            topBannerShownPos = topBannerPanel.anchoredPosition;
            topBannerHiddenPos = new Vector2(topBannerShownPos.x, topBannerShownPos.y + 300f);
            topBannerPanel.anchoredPosition = topBannerHiddenPos;
        }

        // 시작 시 패널들이 숨겨져 있으므로 클릭 등 상호작용 차단
        SetPanelsInteractable(false);
    }

    // 기존 기능 유지 ('E' 키 안내 텍스트)
    public void SetText(string text)
    {
        if (uiText != null && !uiText.gameObject.activeInHierarchy) uiText.gameObject.SetActive(true);
        if (uiText != null) uiText.text = text;
    }

    public void HideText()
    {
        if (uiText != null && uiText.gameObject.activeInHierarchy) uiText.gameObject.SetActive(false);
    }

    // 패널 띄우고 데이터 채우기
    public void ShowDetailPanels(CharacterType type)
    {
        // 패널이 나타나기 시작하면 클릭 등 상호작용 활성화
        SetPanelsInteractable(true);

        if (escHandler != null) escHandler.gameObject.SetActive(true);

        CharacterData data = PlayerManager.Instance.GetCharacterData(type);

        if (characterNameText != null) characterNameText.text = type.ToString();
        if (backgroundStoryText != null) backgroundStoryText.text = data.backgroundStory;

        if (characterIllustrationImage != null)
        {
            if (data.illustration != null)
            {
                characterIllustrationImage.sprite = data.illustration;
                characterIllustrationImage.gameObject.SetActive(true);
            }
            else
            {
                characterIllustrationImage.gameObject.SetActive(false);
            }
        }

        if (skillIconImage != null && data.skillIcon != null)
            skillIconImage.sprite = data.skillIcon;

        if (skillDescriptionText != null) skillDescriptionText.text = data.skillDescription;

        if (skillVideoPlayer != null && data.skillPreviewVideo != null)
        {
            skillVideoPlayer.clip = data.skillPreviewVideo;
            skillVideoPlayer.Play();
        }

        if (leftPanel != null && rightPanel != null)
        {
            if (panelCoroutine != null) StopCoroutine(panelCoroutine);
            panelCoroutine = StartCoroutine(SlidePanels(leftPanelShownPos, rightPanelShownPos));
        }
    }

    public void HideDetailPanels()
    {
        // 숨기기 명령이 떨어지는 즉시 패널의 상호작용 완벽 차단 방어
        SetPanelsInteractable(false);

        if (escHandler != null && escHandler.gameObject.activeSelf)
        {
            escHandler.CloseManually();
        }

        if (skillVideoPlayer != null) skillVideoPlayer.Stop();

        // 선택 취소 시 열려있는 확대 창들 모두 안전하게 닫기
        if (largeVideoPanel != null) largeVideoPanel.SetActive(false);
        if (largeIllustrationPanel != null) largeIllustrationPanel.SetActive(false);

        if (leftPanel != null && rightPanel != null)
        {
            if (panelCoroutine != null) StopCoroutine(panelCoroutine);
            panelCoroutine = StartCoroutine(SlidePanels(leftPanelHiddenPos, rightPanelHiddenPos));
        }
    }

    public void OpenLargeVideo()
    {
        // 영상이 있을 때만 확대 창을 켬
        if (largeVideoPanel != null && skillVideoPlayer.clip != null)
        {
            largeVideoPanel.SetActive(true);
        }
    }

    public void CloseLargeVideo()
    {
        if (largeVideoPanel != null)
        {
            largeVideoPanel.SetActive(false);
        }
    }

    public void OpenLargeIllustration()
    {
        if (largeIllustrationPanel != null && characterIllustrationImage != null && characterIllustrationImage.sprite != null)
        {
            if (largeIllustrationImage != null)
            {
                largeIllustrationImage.sprite = characterIllustrationImage.sprite;
            }
            largeIllustrationPanel.SetActive(true);
        }
    }

    public void CloseLargeIllustration()
    {
        if (largeIllustrationPanel != null) largeIllustrationPanel.SetActive(false);
    }

    public void ShowTopBanner()
    {
        if (topBannerPanel == null) return;
        if (bannerCoroutine != null) StopCoroutine(bannerCoroutine);
        bannerCoroutine = StartCoroutine(SlideBanner(topBannerShownPos));
    }

    public void HideTopBanner()
    {
        if (topBannerPanel == null) return;
        if (bannerCoroutine != null) StopCoroutine(bannerCoroutine);
        bannerCoroutine = StartCoroutine(SlideBanner(topBannerHiddenPos));
    }

    private IEnumerator SlidePanels(Vector2 leftTarget, Vector2 rightTarget)
    {
        float elapsed = 0f;
        Vector2 leftStart = leftPanel.anchoredPosition;
        Vector2 rightStart = rightPanel.anchoredPosition;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            t = t * (2f - t); // 부드러운 감속 효과 (Ease Out)

            leftPanel.anchoredPosition = Vector2.Lerp(leftStart, leftTarget, t);
            rightPanel.anchoredPosition = Vector2.Lerp(rightStart, rightTarget, t);

            yield return null;
        }

        leftPanel.anchoredPosition = leftTarget;
        rightPanel.anchoredPosition = rightTarget;
    }

    private IEnumerator SlideBanner(Vector2 targetPos)
    {
        float elapsed = 0f;
        Vector2 startPos = topBannerPanel.anchoredPosition;

        while (elapsed < bannerSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bannerSlideDuration;
            t = t * (2f - t);

            topBannerPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        topBannerPanel.anchoredPosition = targetPos;
    }

    private void SetPanelsInteractable(bool isInteractable)
    {
        if (leftPanel != null)
        {
            CanvasGroup leftGroup = leftPanel.GetComponent<CanvasGroup>();
            if (leftGroup == null) leftGroup = leftPanel.gameObject.AddComponent<CanvasGroup>();

            leftGroup.interactable = isInteractable;
            leftGroup.blocksRaycasts = isInteractable;
        }

        if (rightPanel != null)
        {
            CanvasGroup rightGroup = rightPanel.GetComponent<CanvasGroup>();
            if (rightGroup == null) rightGroup = rightPanel.gameObject.AddComponent<CanvasGroup>();

            rightGroup.interactable = isInteractable;
            rightGroup.blocksRaycasts = isInteractable;
        }
    }
}