using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    private static PlayerStatusUI instance;
    public static PlayerStatusUI Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PlayerStatusUI>();
            return instance;
        }
    }

    [Header("HP & Gold")]
    [SerializeField] private Slider hpBarSlider;
    [SerializeField] private Text hpTxt;
    [SerializeField] private Text goldText;

    [Header("EXP & Level")]
    [SerializeField] private Slider expBar;
    [SerializeField] private Text levelText;
    [SerializeField] private Text expText;

    [Header("Portrait")]
    [SerializeField] private Image playerHead;

    private PlayerStatus playerStatus;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerCharacterChanged.AddListener(OnPlayerChanged);
            if (PlayerManager.Instance.CurrentPlayer != null) OnPlayerChanged();
        }
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnPlayerCharacterChanged.RemoveListener(OnPlayerChanged);
    }

    public void OnPlayerChanged()
    {
        StopAllCoroutines();
        StartCoroutine(InitPlayerStatus());
    }

    private IEnumerator InitPlayerStatus()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null && PlayerManager.Instance.CurrentPlayer != null);

        playerStatus = PlayerManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        while (playerStatus == null)
        {
            yield return null;
            if (PlayerManager.Instance.CurrentPlayer != null)
                playerStatus = PlayerManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        }

        // CharacterDatabase에서 초상화(Portrait Icon) 가져오기
        if (playerHead != null)
        {
            CharacterType currentType = PlayerManager.Instance.CurrentCharacterType;
            CharacterData data = PlayerManager.Instance.GetCharacterData(currentType);
            playerHead.sprite = data.portraitIcon;
        }

        CheckGold();
        UpdateHpSmooth(playerStatus.Hp, playerStatus.MaxHp);

        int currentLevel = (int)playerStatus.Level;
        int maxExp = 100;

        if (LevelUp.requiredExp != null && LevelUp.requiredExp.Length > currentLevel)
            maxExp = LevelUp.requiredExp[currentLevel];

        UpdateLevelUI(currentLevel);
        UpdateExpUI(playerStatus.Exp, maxExp);
    }

    public void UpdateHpSmooth(float targetHp, float maxHp)
    {
        StartCoroutine(SmoothHpBar(targetHp, maxHp));
    }

    private IEnumerator SmoothHpBar(float targetHp, float maxHp)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        float startValue = hpBarSlider.value;
        float targetValue = (float)targetHp / maxHp;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            if (hpBarSlider != null) hpBarSlider.value = currentValue;
            if (hpTxt != null) hpTxt.text = $"{(int)(currentValue * maxHp)}/{(int)maxHp}";
            yield return null;
        }

        if (hpBarSlider != null) hpBarSlider.value = targetValue;
        if (hpTxt != null) hpTxt.text = $"{(int)targetHp}/{(int)maxHp}";
    }

    public void UpdateExpUI(float currentExp, float maxExp)
    {
        if (expBar != null) expBar.value = (maxExp > 0) ? (currentExp / maxExp) : 0;
        if (expText != null) expText.text = $"{(int)currentExp} / {(int)maxExp}";
    }

    public void UpdateLevelUI(int level)
    {
        if (levelText != null) levelText.text = "Lv." + level;
    }

    public void CheckGold()
    {
        if (playerStatus != null && goldText != null)
        {
            goldText.text = playerStatus.Gold.ToString();
        }
    }
}