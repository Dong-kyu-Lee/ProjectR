using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject blacksmithUI;
    [SerializeField] private GameObject bartenderUI;

    [SerializeField] private SkillCoolTime skillCoolTimeManager;

    private Dictionary<CharacterType, GameObject> uiMap;

    [SerializeField] private SkillIcon skillIcon;

    void Awake()
    {
        uiMap = new Dictionary<CharacterType, GameObject>
        {
            { CharacterType.Blacksmith, blacksmithUI },
            { CharacterType.Bartender, bartenderUI }
        };

        foreach (var kvp in uiMap)
        {
            kvp.Value.SetActive(false);
        }
    }

    private void Start()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerCharacterChanged.AddListener(InitUIForCurrentPlayer);
        }

        InitUIForCurrentPlayer();
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerCharacterChanged.RemoveListener(InitUIForCurrentPlayer);
        }
    }

    // 매개변수 string을 CharacterType으로 변경
    public void SetActiveUI(CharacterType characterType, IAbilityV2 ability)
    {
        // 모든 UI 비활성화
        foreach (var kvp in uiMap)
        {
            kvp.Value.SetActive(false);
        }

        // 해당 캐릭터 UI 활성화
        if (uiMap.TryGetValue(characterType, out GameObject targetUI))
        {
            targetUI.SetActive(true);

            var abilityUI = targetUI.GetComponent<AbilityUIBase>();
            abilityUI?.BindAbility(ability);
            abilityUI.gameObject.SetActive(true);
            Debug.Log("활성화:" + abilityUI.name);
        }
        else
        {
            Debug.LogWarning("해당 UI 프리팹 없음");
        }

        if (skillCoolTimeManager != null)
        {
            CharacterData data = PlayerManager.Instance.GetCharacterData(characterType);

            // 데이터베이스에 아이콘이 할당되어 있을 때만 적용 (안전장치)
            if (data.skillIcon != null)
            {
                skillCoolTimeManager.SetSkillIcon(data.skillIcon);
            }
            skillCoolTimeManager.ResetCooldownUI();
        }
    }

    public void InitUIForCurrentPlayer()
    {
        StopAllCoroutines();
        StartCoroutine(WaitAndBind());
    }

    private IEnumerator WaitAndBind()
    {
        yield return new WaitUntil(() =>
        {
            var player = PlayerManager.Instance.CurrentPlayer;
            if (player == null) return false;
            var controller = player.GetComponent<PlayerControllerBase>();
            return controller != null && controller.GetCharacterAbility() != null;
        });

        var player = PlayerManager.Instance.CurrentPlayer;
        var controller = player.GetComponent<PlayerControllerBase>();

        CharacterType currentType = PlayerManager.Instance.CurrentCharacterType;
        SetActiveUI(currentType, controller.GetCharacterAbility());
    }
}