using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class EndSceneManager : MonoBehaviour
{
    [Header("배경 움직임 설정")]
    public float backgroundSpeed;
    public float frontBackgroundSpeed;
    public GameObject background;
    public GameObject frontBackground;
    [Header("결과 화면 설정")]
    public GameObject resultBackground;
    public TextMeshProUGUI exitText;
    [Header("Contents")]
    public TextMeshProUGUI statusText;
    public GameObject itemGrid;
    public int killCount;
    public float maximumDamage;
    public int playTime;

    [SerializeField]
    private Image[] itemImages;
    private Color original;

    private void Awake()
    {
        
        original = exitText.color;
        // itemImage 초기화
        itemImages = itemGrid.GetComponentsInChildren<Image>();
        CehckField();
    }

    private void Start()
    {
        
    }

    void Update()
    {
        // 배경 오브젝트 움직임
        if (background.transform.position.x <= -20)
        {
            background.transform.position = new Vector3(0, 3, 0);
        }
        else
        {
            background.transform.Translate(Vector3.left * backgroundSpeed * Time.deltaTime);
        }
        if (frontBackground.transform.position.x <= -20)
        {
            frontBackground.transform.position = new Vector3(0, 3, 0);
        }
        else
        {
            frontBackground.transform.Translate(Vector3.left * frontBackgroundSpeed * Time.deltaTime);
        }
    }

    private void CehckField()
    {
        if(background == null) background = GameObject.Find("Background");
        if(frontBackground == null) frontBackground = GameObject.Find("FrontBackground");
        if (itemImages.Length <= 0) Debug.LogError("아이템 이미지 슬롯이 없습니다.");
    }

    // 게임 종료 함수: 동아리의밤에서만 쓸 함수
    public void EndGame()
    {
        GameManager.Instance.MoveScene(SceneType.StartScene, "StartScene");
    }

    // 마우스 포인터가 로비 버튼 위에 올라갔을 때, 노란색으로 하이라이트
    public void OnPointerEnterLobbyButton()
    {
        exitText.color = Color.yellow;
    }
    // 마우스 포인터가 로비 버튼에서 벗어났을 때, 원래 색상으로 복원
    public void OnPointerExitLobbyButton()
    {
        exitText.color = original;
    }
}
