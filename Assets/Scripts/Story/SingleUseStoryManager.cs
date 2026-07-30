using UnityEngine;

// 일회성 스토리(프롤로그 등)의 완료 여부를 json에 영구 기록/조회한다.
// 저장 데이터는 "완료된 일회성 스토리 집합"이며, 항목이 없으면 미완료로 간주한다.
// (기본값을 미리 심어둘 필요가 없어 일회성 스토리를 추가해도 기존 저장 파일과 호환된다.)
public class SingleUseStoryManager
{
    private SingleUseStory data;

    public SingleUseStoryManager()
    {
        data = DataManager.LoadSingleUseStoryData();
    }

    // 해당 일회성 스토리를 이미 완료했는지 여부
    public bool IsCompleted(StoryID storyID)
    {
        var story = data.stories.Find(s => s.name == storyID.ToString());
        return story != null && story.value;
    }

    // 일회성 스토리 완료 처리 후 즉시 저장
    public void Complete(StoryID storyID)
    {
        var story = data.stories.Find(s => s.name == storyID.ToString());
        if (story == null)
        {
            story = new StoryData { name = storyID.ToString() };
            data.stories.Add(story);
        }
        if (story.value) return; // 이미 기록됨 -> 불필요한 파일 쓰기 방지

        story.value = true;
        Debug.Log($"Single-use story '{storyID}' marked as completed.");
        DataManager.SaveSingleUseStoryData(data);
    }
}
