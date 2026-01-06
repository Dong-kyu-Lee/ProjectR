using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    public float backgroundSpeed;
    public float frontBackgroundSpeed;
    public GameObject background;
    public GameObject frontBackground;
    // 게임 최초 시작 시 생성할 플레이어 캐릭터 타입
    public CharacterType initialCharacterType;

    [SerializeField] private TextMeshProUGUI startBtnText;
    [SerializeField] private TextMeshProUGUI exitBtnText;
    private Color original = Color.black;

    private void Start()
    {
        GameManager.Sound.Play("Sounds/BGM/StartSceneBGM", Sound.Bgm);
    }

    void Update()
    {
        // 배경 오브젝트 움직임
        if(background.transform.position.x <= -24.7)
        {
            background.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            background.transform.Translate(Vector3.left * backgroundSpeed * Time.deltaTime);
        }
        if(frontBackground.transform.position.x <= -24.7)
        {
            frontBackground.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            frontBackground.transform.Translate(Vector3.left * frontBackgroundSpeed * Time.deltaTime);
        }
    }

    // 게임 시작 화면에서 Start 버튼에 의해 호출되는 이벤트 함수
    public void StartGame()
    {
        // 첫 시작 시 플레이어 오브젝트 생성
        // GameManager.Instance.CreateFirstPlayer(initialCharacterType);
        GameManager.Instance.MoveScene(SceneType.LobbyScene, "LobbyScene");
        //StartCoroutine(MoveSceneAfterPlayerGenerated());
    }

    // 게임 종료 버튼에 의해 호출되는 이벤트 함수
    public void ExitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    // 마우스 포인터가 로비 버튼 위에 올라갔을 때, 노란색으로 하이라이트
    public void OnPointerEnterStartButton()
    {
        startBtnText.color = Color.yellow;
    }
    // 마우스 포인터가 로비 버튼에서 벗어났을 때, 원래 색상으로 복원
    public void OnPointerExitStartButton()
    {
        startBtnText.color = original;
    }

    public void OnPointerEnterExitButton()
    {
        exitBtnText.color = Color.yellow;
    }
    public void OnPointerExitExitButton()
    {
        exitBtnText.color = original;
    }
}
