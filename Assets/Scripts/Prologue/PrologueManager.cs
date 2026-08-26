using UnityEngine;

public class PrologueManager : MonoBehaviour
{
    // 프롤로그 컷씬이 완료되었을 때 호출되는 함수
    // 완료 기록(json)과 로비 이동은 StorySystem이 일회성 스토리 규칙에 따라 처리한다.
    public void CompleteCutScene()
    {
        StorySystem.Instance.CompleteStory(StoryID.Prologue);
    }
}
