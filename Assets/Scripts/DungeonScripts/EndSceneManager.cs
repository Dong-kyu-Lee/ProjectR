using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public TextMeshProUGUI killCount;
    public TextMeshProUGUI maximumDamage;
    public TextMeshProUGUI playTime;

    [SerializeField]
    private GameObject[] itemImages;
    private Color original;
    private string statusFormat = "{0}\n\n{1}\n\n{2}\n\n{3}\n\n{4}\n\n{5}\n\n{6}";
    // 0 : Level, 1 : Damage, 2. Additional Damage, 3. Critical Percent, 4. Critical Damage
    // 5. Damage Reduction, 6. Attack Speed

    private void Awake()
    {
        original = exitText.color;
        CheckField();
    }

    private void Start()
    {
        Inventory currentInv = GameManager.Instance.CurrentPlayer.GetComponentInChildren<Inventory>();
        if (currentInv == null) { Debug.Log("End SceneManager: currentInv is null"); return; }
        // Inventory에 장착된 장비 이미지를 결과 화면에 표시
        if (currentInv.EquipmentItemSlot.Length > 0)
        {
            for(int i = 0; i < currentInv.EquipmentItemSlot.Length; i++)
            {
                if(i >= itemImages.Length) break;
                if(currentInv.EquipmentItemSlot[i] == null) { Debug.Log("End SceneManager: EquipmentItemSlot[" + i + "] is null"); continue; }
                if (itemImages[i].GetComponent<Image>() == null) { Debug.Log("End SceneManager: itemImages[" + i + "] Image component is null"); continue; }
                itemImages[i].GetComponent<Image>().sprite = currentInv.EquipmentItemSlot[i].ItemSprite;
            }
        }
        // 스테이터스 텍스트 설정
        PlayerStatus playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        if (playerStatus != null)
        {
            statusText.text = string.Format(statusFormat,
                playerStatus.Level,
                playerStatus.TotalDamage.ToString("F1"),
                (playerStatus.AdditionalDamage * 100).ToString("F1") + "%",
                (playerStatus.CriticalPercent * 100).ToString("F1") + "%",
                (playerStatus.CriticalDamage * 100).ToString("F1") + "%",
                (playerStatus.DamageReduction * 100).ToString("F1") + "%",
                playerStatus.TotalAttackSpeed.ToString("F2") + " s"
                );
        }
        // 기타 결과 변수 설정
        killCount.text = GameManager.Instance.totalKillCount.ToString();
        maximumDamage.text = GameManager.Instance.maximumDamage.ToString();
        playTime.text = GameManager.Instance.totalPlayTimeInSeconds.ToString(@"hh\:mm\:ss");
        // BGM 재생
        GameManager.Sound.Play("Sounds/BGM/EndSceneBGM", Sound.Bgm);
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

    private void CheckField()
    {
        if(background == null) background = GameObject.Find("Background");
        if(frontBackground == null) frontBackground = GameObject.Find("FrontBackground");
        if (itemImages.Length <= 0) Debug.LogError("아이템 이미지 슬롯이 없습니다.");
    }

    // 로비 씬으로 돌아가기
    public void BackToLobby()
    {
        // 스테이지 흐름 초기화
        DungeonFlowManager.Instance.ResetStages();
        GameManager.Instance.MoveScene(SceneType.LobbyScene, "LobbyScene");
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
