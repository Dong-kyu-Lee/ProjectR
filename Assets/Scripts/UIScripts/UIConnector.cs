using UnityEngine;

public class UIConnector : MonoBehaviour
{
    [SerializeField] private InGameUIManager inGameUIManager;
    [SerializeField] private CharacterInfo characterInfo;

    private void Awake()
    {
        if (inGameUIManager == null)
            inGameUIManager = GetComponentInChildren<InGameUIManager>(true);

        if (characterInfo == null)
            characterInfo = GetComponentInChildren<CharacterInfo>(true);

        if (inGameUIManager != null && characterInfo != null)
        {
            inGameUIManager.SetCharacterInfoUI(characterInfo);
            Debug.Log("[UIConnector] UI 연결 성공");
        }
        else
        {
            Debug.LogError("[UIConnector] UI 연결 실패.");
        }
    }
}