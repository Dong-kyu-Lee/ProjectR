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

    [Header("UI Stack System")]
    [SerializeField] private CharacterSelectEscHandler escHandler;

    [Header("좌측 패널 UI 요소 (스토리)")]
    [SerializeField] private Text characterNameText;
    [SerializeField] private Text backgroundStoryText;
    [SerializeField] private Image characterIllustrationImage;

    [Header("우측 패널 UI 요소 (스킬)")]
    [SerializeField] private Image skillIconImage;
    [SerializeField] private Text skillDescriptionText;
    [SerializeField] private VideoPlayer skillVideoPlayer; // 영상이 없다면 비워둬도 됨

    private Vector2 leftPanelHiddenPos;
    private Vector2 leftPanelShownPos;
    private Vector2 rightPanelHiddenPos;
    private Vector2 rightPanelShownPos;

    private Coroutine panelCoroutine;

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

        if (leftPanel != null && rightPanel != null)
        {
            if (panelCoroutine != null) StopCoroutine(panelCoroutine);
            panelCoroutine = StartCoroutine(SlidePanels(leftPanelHiddenPos, rightPanelHiddenPos));
        }
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
}