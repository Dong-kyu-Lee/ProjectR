using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitHandler : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite bartenderSprite;
    [SerializeField] private Sprite blacksmithSprite;

    // CharacterSelect에서 호출할 함수
    public void ChangePortrait(CharacterType type)
    {
        if (portraitImage == null) portraitImage = GetComponent<Image>();

        switch (type)
        {
            case CharacterType.Bartender:
                portraitImage.sprite = bartenderSprite;
                break;
            case CharacterType.Blacksmith:
                portraitImage.sprite = blacksmithSprite;
                break;
        }
    }
}