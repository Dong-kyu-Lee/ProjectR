using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
    [SerializeField] private float slideDuration = 0.3f;

    // [신규 추가] 상단 안내 배너 UI
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

    private Vector2 leftPanelHiddenPos;
    private Vector2 leftPanelShownPos;
    private Vector2 rightPanelHiddenPos;
    private Vector2 rightPanelShownPos;

    // 배너 위치 기억용 변수
    private Vector2 topBannerHiddenPos;
    private Vector2 topBannerShownPos;

    private Coroutine panelCoroutine;
    private Coroutine bannerCoroutine; // 배너 코루틴

    void Start()
    {
        if (instance == null) instance = this;
        if (uiText == null) uiText = GetComponentInChildren<TextMeshProUGUI>();

        if (leftPanel != null && rightPanel != null)
        {
            leftPanelShownPos = leftPanel.anchoredPosition;
            leftPanelHiddenPos = new Vector2(leftPanelShownPos.x - 1000f, leftPanelShownPos.y);
            leftPanel.anchoredPosition = leftPanelHiddenPos;

            rightPanelShownPos = rightPanel.anchoredPosition;
            rightPanelHiddenPos = new Vector2(rightPanelShownPos.x + 1000f, rightPanelShownPos.y);
            rightPanel.anchoredPosition = rightPanelHiddenPos;
        }

        // [신규 추가] 상단 배너 초기 위치 설정
        if (topBannerPanel != null)
        {
            topBannerShownPos = topBannerPanel.anchoredPosition;
            // Y축으로 300만큼 올려서 화면 밖으로 숨김 (해상도에 따라 수치 조절 가능)
            topBannerHiddenPos = new Vector2(topBannerShownPos.x, topBannerShownPos.y + 300f);
            topBannerPanel.anchoredPosition = topBannerHiddenPos;
        }
    }

    public void SetText(string text)
    {
        if (uiText != null && !uiText.gameObject.activeInHierarchy) uiText.gameObject.SetActive(true);
        if (uiText != null) uiText.text = text;
    }

    public void HideText()
    {
        if (uiText != null && uiText.gameObject.activeInHierarchy) uiText.gameObject.SetActive(false);
    }

    public void ShowDetailPanels(CharacterType type)
    {
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
        if (escHandler != null && escHandler.gameObject.activeSelf)
        {
            escHandler.CloseManually();
        }

        if (skillVideoPlayer != null) skillVideoPlayer.Stop();
        if (largeVideoPanel != null) largeVideoPanel.SetActive(false);

        if (leftPanel != null && rightPanel != null)
        {
            if (panelCoroutine != null) StopCoroutine(panelCoroutine);
            panelCoroutine = StartCoroutine(SlidePanels(leftPanelHiddenPos, rightPanelHiddenPos));
        }
    }

    public void OpenLargeVideo()
    {
        if (largeVideoPanel != null && skillVideoPlayer.clip != null)
        {
            largeVideoPanel.SetActive(true);
        }
    }

    public void CloseLargeVideo()
    {
        if (largeVideoPanel != null) largeVideoPanel.SetActive(false);
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
            t = t * (2f - t);

            leftPanel.anchoredPosition = Vector2.Lerp(leftStart, leftTarget, t);
            rightPanel.anchoredPosition = Vector2.Lerp(rightStart, rightTarget, t);

            yield return null;
        }

        leftPanel.anchoredPosition = leftTarget;
        rightPanel.anchoredPosition = rightTarget;
    }

    // 상단 배너 슬라이딩 함수
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
}