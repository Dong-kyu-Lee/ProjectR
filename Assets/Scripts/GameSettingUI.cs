using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingUI : MonoBehaviour
{
    Animator animator;
    [SerializeField] GameObject background;
    [SerializeField] GameObject fakePanel1;
    [SerializeField] GameObject fakePanel2;
    [SerializeField] GameObject Panel;
    Color original;
    bool isOpen = false;

    [Header("Sound Setting UI")]
    public TextMeshProUGUI lobbyButtonText;
    public TextMeshProUGUI exitButtonText;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle fullScreenToggle;

    void Awake()
    {
        if(FindObjectOfType<GameSettingUI>() != this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        animator = GetComponent<Animator>();
        original = lobbyButtonText.color;
    }

    // 설정 UI 열기/닫기 함수
    public void OpenCloseSettingUI()
    {
        if(isOpen == false)
        {
            animator.gameObject.SetActive(true);
            animator.SetBool("isOpen", true);
            background.SetActive(true);
            fakePanel1.SetActive(true);
            fakePanel2.SetActive(true);
            Panel.SetActive(true);
            // 현재 BGM 및 SFX 볼륨 값을 슬라이더에 반영
            bgmSlider.value = GameManager.Sound.GetBgmVolume();
            sfxSlider.value = GameManager.Sound.GetEffectVolume();
            fullScreenToggle.isOn = Screen.fullScreen;
            isOpen = true;
        }
        else
        {
            animator.SetBool("isOpen", false);
            background.SetActive(false);
            fakePanel1.SetActive(false);
            fakePanel2.SetActive(false);
            Panel.SetActive(false);
            Time.timeScale = 1f;
            isOpen = false;
        }
    }

    #region Lobby Button Event Handlers
    // 로비 버튼 클릭 시 로비 씬으로 이동
    public void OnClickLobbyButton()
    {
        animator.SetBool("isOpen", false);
        background.SetActive(false);
        fakePanel1.SetActive(false);
        fakePanel2.SetActive(false);
        Panel.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;

        if (SceneManager.GetActiveScene().name != "LobbyScene")
        {
            GameManager.Instance.PlayTimeStop();
            GameManager.Instance.PlayerDead();
        }
    }
    // 마우스 포인터가 로비 버튼 위에 올라갔을 때, 노란색으로 하이라이트
    public void OnPointerEnterLobbyButton()
    {
        lobbyButtonText.color = Color.yellow;
    }
    // 마우스 포인터가 로비 버튼에서 벗어났을 때, 원래 색상으로 복원
    public void OnPointerExitLobbyButton()
    {
        lobbyButtonText.color = original;
    }
    #endregion

    #region Exit Button Event Handlers
    // 종료 버튼 클릭 시 애플리케이션 종료
    public void OnClickExitButton()
    {
        Application.Quit();
    }
    // 마우스 포인터가 종료 버튼 위에 올라갔을 때, 노란색으로 하이라이트
    public void OnPointerEnterExitButton()
    {
        exitButtonText.color = Color.yellow;
    }
    // 마우스 포인터가 종료 버튼에서 벗어났을 때, 원래 색상으로 복원
    public void OnPointerExitExitButton()
    {
        exitButtonText.color = original;
    }
    #endregion

    // BGM 및 SFX 볼륨 슬라이더 값 변경 시 호출되는 함수
    public void OnBGMVolumeChanged()
    {
        GameManager.Sound.SetBgmVolume(bgmSlider.value);
    }

    public void OnSFXVolumeChanged()
    {
        GameManager.Sound.SetEffectVolume(sfxSlider.value);
    }

    // 전체 화면 여부를 나타내는 토글 값 변경 시 호출되는 함수(true: 전체 화면, false: 창 모드)
    public void OnFullScreenToggleChanged()
    {
        Screen.fullScreen = fullScreenToggle.isOn;
    }

    // 애니메이션이 끝났을 때 GameSettingUI Animation event에 의해 호출되는 함수
    public void OnAnimationEnd()
    {
        Time.timeScale = 0f;
    }
}
