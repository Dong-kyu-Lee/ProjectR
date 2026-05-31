using UnityEngine;
using UnityEngine.UI;

public class BuffToolTipUI : MonoBehaviour
{
    [SerializeField]
    private Image buffImage;
    [SerializeField]
    private Text buffNameText;
    [SerializeField]
    private Text buffDescriptionText;

    private bool isVisible = false;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        isVisible = false;
    }

    public void ShowTooltip(Buff buff, Sprite sprite)
    {
        if (buff == null) return;

        gameObject.SetActive(true);
        buffNameText.text = buff.BuffType.ToString();

        // [수정/개선] 길었던 switch문 대신 Helper 클래스에게 설명을 요청합니다.
        buffDescriptionText.text = BuffUIDataHelper.GetDescription(buff);

        buffImage.sprite = sprite;
        isVisible = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isVisible = false;
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}