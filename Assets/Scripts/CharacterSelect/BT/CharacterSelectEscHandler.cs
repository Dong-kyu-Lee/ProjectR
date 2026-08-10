using UnityEngine;

// 이 스크립트를 붙이면 기존의 EscClosable 스크립트도 자동으로 함께 붙습니다.
[RequireComponent(typeof(EscClosable))]
public class CharacterSelectEscHandler : MonoBehaviour
{
    private bool isManualClose = false;

    // 닫기/선택 버튼을 눌러서 '정상적'으로 닫을 때 호출할 함수
    public void CloseManually()
    {
        isManualClose = true;
        gameObject.SetActive(false); // 스택에서 안전하게 제거됨
    }

    private void OnDisable()
    {
        // 만약 버튼(CloseManually)을 누른 게 아닌데 이 오브젝트가 꺼졌다면?
        // = UI 스택 매니저가 ESC 키 입력을 받아 강제로 껐다는 의미!
        if (!isManualClose && CharacterSelectManager.Instance != null)
        {
            // 매니저에게 취소(Cancel) 명령을 내려서 줌아웃과 패널 슬라이딩 연출을 정상 실행시킴
            CharacterSelectManager.Instance.CancelSelection();
        }

        isManualClose = false; // 다음을 위해 상태 초기화
    }
}