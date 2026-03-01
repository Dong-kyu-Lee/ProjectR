using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인게임 미니맵에 표시될 모든 오브젝트에 부착되는 스크립트
// 현재 위치, 상태를 미니맵에 표시
public class MinimapIconUI : MonoBehaviour
{
    [SerializeField]
    GameObject minimapIconObject;
    public Sprite iconSprite;
    public float iconScale = 1.0f;

    void Start()
    {
        if(iconSprite == null)
        {
            Debug.LogError("MinimapIconUI: Icon Sprite is not assigned.");
            return;
        }

        minimapIconObject = new GameObject("MinimapIcon");
        minimapIconObject.transform.SetParent(this.transform);
        minimapIconObject.layer = LayerMask.NameToLayer("Minimap");
        minimapIconObject.transform.localPosition = Vector3.zero;
        minimapIconObject.transform.localScale = Vector3.one * iconScale;
        SpriteRenderer sr = minimapIconObject.AddComponent<SpriteRenderer>();
        sr.sprite = iconSprite;
    }
}
