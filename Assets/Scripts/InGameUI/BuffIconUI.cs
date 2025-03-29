using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BuffIconUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private Text buffLevelText;

    // 버프 타입에 따른 스프라이트 매핑 (Inspector를 통해 할당하면 좋습니다)
    [SerializeField] private List<BuffSpriteEntry> buffSpriteEntries = new List<BuffSpriteEntry>();
    // 내부적으로 딕셔너리로 변환해 사용 가능
    private Dictionary<BuffType, Sprite> buffSpriteDictionary = new Dictionary<BuffType, Sprite>();

    private Buff buffData;

    private void Awake()
    {
        // 리스트를 딕셔너리로 변경 - Inspector에서 할당한 값들을 기반으로 함
        foreach (var entry in buffSpriteEntries)
        {
            if (!buffSpriteDictionary.ContainsKey(entry.buffType))
                buffSpriteDictionary.Add(entry.buffType, entry.sprite);
        }
    }

    public void Initialize(Buff newBuffData)
    {
        buffData = newBuffData;
        UpdateUI();
    }

    public void UpdateData(Buff newBuffData)
    {
        buffData = newBuffData;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (buffData != null)
        {
            if (cooldownOverlay != null && buffData.MaxDuration > 0)
            {
                cooldownOverlay.fillAmount = buffData.CurrentDuration / buffData.MaxDuration;
            }
            if (buffLevelText != null)
            {
                buffLevelText.text = (buffData.CurrentBuffLevel + 1).ToString();
            }
            // 버프 타입에 맞는 스프라이트 적용 (만약 매핑된 이미지가 존재하면)
            if (iconImage != null && buffSpriteDictionary.ContainsKey(GetBuffType(buffData)))
            {
                iconImage.sprite = buffSpriteDictionary[GetBuffType(buffData)];
            }
        }
    }

    // 여기서 buffData에서 버프 타입을 결정하는 방법은
    // 예를 들어 buffData가 실제 BuffType이라는 정보를 갖고 있다면 그것을 반환하도록 합니다.
    // 이 예시는 단순한 예제용입니다.
    private BuffType GetBuffType(Buff buffData)
    {
        // 예를 들면, buffData.GetType()에 따라 특정 BuffType을 매핑하는 로직을 넣을 수 있습니다.
        // 현재 단순히 캐스팅하는 형태로 가정합니다.
        // 실제 코드는 buffData 내부에 BuffType 필드를 추가하거나, 별도의 변환 로직을 구현해야 합니다.
        return (BuffType)System.Enum.Parse(typeof(BuffType), buffData.GetType().Name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowBuffDetail();
    }

    private void ShowBuffDetail()
    {
        Debug.Log("버프 상세 정보를 표시합니다: " + buffData.GetType().Name);
        // 상세 정보 UI 로직 구현...
    }
}

// Inspector에서 할당할 데이터 구조체 (Serializable)
[System.Serializable]
public struct BuffSpriteEntry
{
    public BuffType buffType;
    public Sprite sprite;
}