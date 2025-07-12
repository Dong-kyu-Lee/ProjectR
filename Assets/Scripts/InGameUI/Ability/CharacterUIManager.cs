using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIManager : MonoBehaviour
{
    [SerializeField] private GameObject blacksmithUI;
    [SerializeField] private GameObject bartenderUI;

    private Dictionary<string, GameObject> uiMap;

    void Awake()
    {
        uiMap = new Dictionary<string, GameObject>
        {
            { "Blacksmith", blacksmithUI },
            { "Bartender", bartenderUI }
        };

        foreach (var kvp in uiMap)
        {
            kvp.Value.SetActive(false);
        }
    }

    private void Start()
    {
        GameManager.Instance.OnPlayerCharacterChanged.AddListener(InitUIForCurrentPlayer);
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
    }

    public void InitUIForCurrentPlayer()
    {
        StartCoroutine(WaitAndBind());
    }

    private IEnumerator WaitAndBind()
    {
        yield return new WaitUntil(() =>
        {
            var player = GameManager.Instance.CurrentPlayer;
            if (player == null) return false;
            var controller = player.GetComponent<PlayerControllerBase>();
            return controller != null && controller.GetCharacterAbility() != null;
        });

        var player = GameManager.Instance.CurrentPlayer;
        var controller = player.GetComponent<PlayerControllerBase>();

        string characterType = player.name.Contains("Blacksmith") ? "Blacksmith" : "Bartender";
        SetActiveUI(characterType, controller.GetCharacterAbility());
    }
}
