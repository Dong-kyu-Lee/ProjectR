using UnityEngine;

public class EscClosable : MonoBehaviour
{
    private void OnEnable()
    {
        // UI가 켜질 때 스택에 추가
        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.RegisterUI(this.gameObject);
        }
    }

    private void OnDisable()
    {
        // UI가 꺼질 때 스택에서 제거
        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.UnregisterUI(this.gameObject);
        }
    }
}