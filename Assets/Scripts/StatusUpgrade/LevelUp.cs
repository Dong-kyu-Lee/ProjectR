using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private UpgradeStatus upgradeStatus;
    private UpgradeSystem upgradeSystem;
    private StatusValueText statusValueText;

    // 요구 경험치 ex) 레벨 1 -> 레벨 2 요구 경험치 = requiredExp[1].
    public static readonly int[] requiredExp;
    [SerializeField] private int maxlevel = 50;

    public static LevelUp Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
            return;
        }
    }

    private void Start()
    {
        StartCoroutine(InitPlayerStatus());
    }

    void OnEnable()
    {
        // 리스너 등록
        PlayerManager.Instance.OnPlayerCharacterChanged.AddListener(ResetPlayerInfo);
    }

    void OnDisable()
    {
        // 리스너 해제 (메모리 누수 방지)
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnPlayerCharacterChanged.RemoveListener(ResetPlayerInfo);
    }

    // 경험치 통 정의.
    static LevelUp()
    {
        int maxLevel = 50; // 최대 레벨.
        requiredExp = new int[maxLevel + 1];

        for (int i = 1; i < maxLevel; i++)
        {
            double exp = 100 + 2 * Math.Pow(i, 2) * Math.Pow(1.03, i);
            requiredExp[i] = (int)Math.Round(exp);
        }
        requiredExp[maxLevel] = 0;
    }

    // 경험치 증가.
    public void IncreaseExp(float value)
    {
        if (playerStatus == null)
        {
            playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
            if (playerStatus == null) return;
        }
        if (playerStatus.Level < maxlevel)
        {
            playerStatus.Exp += value;

            if (InGameUIManager.Instance != null)
            {
                int maxExp = requiredExp[(int)playerStatus.Level];
                InGameUIManager.Instance.UpdateExpUI(playerStatus.Exp, maxExp);
            }
            else
            {
                Debug.Log("InGameUIManager가 할당되지 않음");
            }

            if (playerStatus.Exp > requiredExp[(int)playerStatus.Level])
            {
                UpLevel();
            }
        }
        else
        {
            playerStatus.Exp = 0;
            if (InGameUIManager.Instance != null)
            {
                InGameUIManager.Instance.UpdateExpUI(0, 1);
            }
        }
    }

    // 레벨 업.
    public void UpLevel()
    {
        var levelUpEffect = GameManager.Instance.CurrentPlayer.GetComponentInChildren<LevelUpEffectSystem>(true);
        if (levelUpEffect != null) levelUpEffect.Play();

        playerStatus.Level++;
        upgradeStatus.StatPoint++;

        playerStatus.Damage += 1;
        playerStatus.MaxHp += 5;
        playerStatus.Hp += 5;

        playerStatus.Exp -= requiredExp[(int)playerStatus.Level - 1];

        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.UpdateLevelUI((int)playerStatus.Level);
            InGameUIManager.Instance.UpdateHpSmooth(playerStatus.Hp, playerStatus.MaxHp);
        }

        if (playerStatus.Level == maxlevel) playerStatus.Exp = 0;
        if (playerStatus.Exp > requiredExp[(int)playerStatus.Level] && playerStatus.Level < maxlevel)
        {
            UpLevel();
        }

        statusValueText.SetupValueText(upgradeStatus);

        if (InGameUIManager.Instance != null && playerStatus.Level < maxlevel)
        {
            int maxExp = requiredExp[(int)playerStatus.Level];
            InGameUIManager.Instance.UpdateExpUI(playerStatus.Exp, maxExp);
        }
    }

    // 레벨 초기화.
    public void ResetLevel()
    {
        upgradeSystem.ResetStat();
        playerStatus.Damage -= playerStatus.Level - 1;
        playerStatus.MaxHp -= (playerStatus.Level - 1) * 5;
        playerStatus.Level = 1;
        playerStatus.Exp = 0;

        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.UpdateLevelUI(1);
            InGameUIManager.Instance.UpdateExpUI(0, requiredExp[1]);
            InGameUIManager.Instance.UpdateHpSmooth(playerStatus.Hp, playerStatus.MaxHp);
        }
    }

    private IEnumerator InitPlayerStatus()
    {
        // 플레이어가 확실히 준비될 때까지 대기
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.CurrentPlayer != null);

        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
        upgradeSystem = transform.GetComponentInChildren<UpgradeSystem>(true);
        statusValueText = transform.GetComponentInChildren<StatusValueText>(true);
        //UI갱신용
        if (InGameUIManager.Instance != null && playerStatus != null)
        {
            int maxExp = requiredExp[(int)playerStatus.Level];
            InGameUIManager.Instance.UpdateExpUI(playerStatus.Exp, maxExp);
            InGameUIManager.Instance.UpdateLevelUI((int)playerStatus.Level);
        }
    }

    public void ResetPlayerInfo()
    {
        playerStatus = GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>();
        upgradeStatus = GameManager.Instance.CurrentPlayer.GetComponent<UpgradeStatus>();
        Debug.Log("캐릭터 정보 초기화");
    }

#if UNITY_EDITOR
    public int MaxLevelForEditor => Mathf.Min(maxlevel, requiredExp.Length - 1);
    public UpgradeSystem UpgradeSystemForEditor => upgradeSystem;

    public bool IsReadyForEditor(PlayerStatus player) =>
        isActiveAndEnabled && player != null && playerStatus == player && upgradeStatus != null
        && statusValueText != null && player.Level >= 1 && player.Level <= MaxLevelForEditor;

    public void GrantExpForEditor(int amount)
    {
        IncreaseExp(amount);
        if (playerStatus.Level >= MaxLevelForEditor)
            InGameUIManager.Instance?.UpdateExpUI(0, 0);
    }

    public void GrowToLevelForEditor(int targetLevel)
    {
        int currentLevel = (int)playerStatus.Level;
        if (targetLevel <= currentLevel || targetLevel > MaxLevelForEditor) return;

        // 기존 레벨업은 요구 경험치를 초과해야 발생한다.
        int totalRequired = 1;
        for (int level = currentLevel; level < targetLevel; level++)
            totalRequired += requiredExp[level];

        GrantExpForEditor(Mathf.Max(1, Mathf.CeilToInt(totalRequired - playerStatus.Exp)));
    }

    public void GrantStatPointsForEditor(int amount)
    {
        upgradeStatus.StatPoint += amount;
        statusValueText.SetupValueText(upgradeStatus);
    }
#endif
}
