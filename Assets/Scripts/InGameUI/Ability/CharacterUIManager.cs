using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject blacksmithUI;
    [SerializeField] private GameObject bartenderUI;

    [Header("Skill Icons")]
    [SerializeField] private Sprite blacksmithIcon;
    [SerializeField] private Sprite bartenderIcon;

    [SerializeField] private SkillCoolTime skillCoolTimeManager;

    private Dictionary<string, GameObject> uiMap;
    private Dictionary<string, Sprite> iconMap;

    void Awake()
    {
        uiMap = new Dictionary<string, GameObject>
        {
            { "Blacksmith", blacksmithUI },
            { "Bartender", bartenderUI }
        };

        iconMap = new Dictionary<string, Sprite>
        {
            { "Blacksmith", blacksmithIcon },
            { "Bartender", bartenderIcon }
        };

        foreach (var kvp in uiMap)
        {
            kvp.Value.SetActive(false);
        }
    }

    private void Start()
    {
        // PlayerManager의 캐릭터 교체 이벤트를 스스로 구독하도록 변경
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

    public void SetActiveUI(string characterType, IAbilityV2 ability)
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

        if (iconMap.TryGetValue(characterType, out Sprite targetIcon))
        {
            if (skillCoolTimeManager != null)
            {
                skillCoolTimeManager.SetSkillIcon(targetIcon); // 아이콘 교체
                skillCoolTimeManager.ResetCooldownUI();        // 쿨타임 UI 초기화
            }
        }
    }

    public void InitUIForCurrentPlayer()
    {
        // 잦은 교체로 인해 코루틴이 여러 개 겹쳐서 실행되는 것을 방지
        StopAllCoroutines();
        StartCoroutine(WaitAndBind());
    }

    private IEnumerator WaitAndBind()
    {
        yield return new WaitUntil(() =>
        {
            // 안전하게 새로운 코어인 PlayerManager를 참조
            var player = PlayerManager.Instance.CurrentPlayer;
            if (player == null) return false;
            var controller = player.GetComponent<PlayerControllerBase>();
            return controller != null && controller.GetCharacterAbility() != null;
        });

        var player = PlayerManager.Instance.CurrentPlayer;
        var controller = player.GetComponent<PlayerControllerBase>();

        string characterType = player.name.Contains("Blacksmith") ? "Blacksmith" : "Bartender";
        SetActiveUI(characterType, controller.GetCharacterAbility());
    }
}