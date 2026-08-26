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
        SoundManager.Instance.Play("Sounds/BGM/StartSceneBGM", Sound.Bgm);
        SoundManager.Instance.SetBgmVolume(GameSettingsSaver.GetBGMValue());
        SoundManager.Instance.SetEffectVolume(GameSettingsSaver.GetSFXValue());
        Screen.fullScreen = GameSettingsSaver.GetFullScreen();
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
        // 프롤로그는 일회성 스토리이므로 재생 여부 판단(json 조회)과 씬 이동을 StorySystem에 맡긴다.
        if (StorySystem.Instance.StartStory(StoryID.Prologue)) return;

        // 이미 프롤로그를 본 경우 곧바로 로비로 이동
        GameManager.Instance.MoveScene(SceneType.LobbyScene, "LobbyScene");
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
