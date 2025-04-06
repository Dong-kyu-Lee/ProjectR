using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuffIconUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private Text buffNameText;
    [SerializeField]
    private Text descriptionText;
    [SerializeField]
    private Image cooldownOverlay;
    [SerializeField]
    private Text buffLevelText;

    private BuffInfo buffInfo; // 데이터 에셋
    private Buff buffData;     // 실제 적용되고 있는 버프 로직

    // 초기화함수: BuffInfo와 Buff 데이터를 함께 받아 UI를 설정합니다.
    public void Initialize(BuffInfo info, Buff data)
    {
        buffInfo = info;
        buffData = data;
        UpdateUI();
    }

    // 업데이트 함수: 시간 변화 등에 따라 UI 갱신 
    public void UpdateUI()
    {
        if (iconImage != null)
            iconImage.sprite = buffInfo.buffIcon;
        if (buffNameText != null)
            buffNameText.text = buffInfo.buffName;
        if (descriptionText != null)
            descriptionText.text = buffInfo.description;
        if (cooldownOverlay != null && buffData.MaxDuration > 0)
            cooldownOverlay.fillAmount = buffData.CurrentDuration / buffData.MaxDuration;
        if (buffLevelText != null)
            buffLevelText.text = "Lv " + (buffData.CurrentBuffLevel + 1).ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 아이콘 클릭 시 상세 정보, 팝업, 툴팁 등 추가 처리 가능
        Debug.Log("버프 상세 정보: " + buffInfo.description);
    }
}