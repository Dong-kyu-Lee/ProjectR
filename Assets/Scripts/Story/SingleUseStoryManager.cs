using UnityEngine;

// 일회성 스토리(프롤로그 등)의 로드/조회/저장만 담당한다.
// StorySystem에서 이 책임을 분리하여 응집도를 높이고, 기본값 키를 한곳에서 관리한다.
public class SingleUseStoryManager
{
    private SingleUseStory data;

    public SingleUseStoryManager()
    {
        data = DataManager.LoadSingleUseStoryData();

        // 저장된 데이터가 없으면 기본 일회성 스토리 목록을 만들어 저장
        if (data.stories.Count == 0)
        {
            data.stories.Add(new StoryData { name = SingleUseStoryKeys.Prologue, value = false });
            data.stories.Add(new StoryData { name = SingleUseStoryKeys.InLobby, value = false });
            DataManager.SaveSingleUseStoryData(data);
        }
    }

    // 해당 일회성 스토리를 이미 완료했는지 여부
    public bool IsCompleted(string storyName)
    {
        var story = data.stories.Find(s => s.name == storyName);
        if (story == null)
        {
            Debug.LogError($"Single-use story '{storyName}' not found.");
            return false;
        }
        return story.value;
    }

    // 일회성 스토리 완료 처리 후 즉시 저장
    public void Complete(string storyName)
    {
        var story = data.stories.Find(s => s.name == storyName);
        if (story == null)
        {
            Debug.LogError($"Single-use story '{storyName}' not found.");
            return;
        }
        story.value = true;
        Debug.Log($"Single-use story '{storyName}' marked as completed.");
        DataManager.SaveSingleUseStoryData(data);
    }
}

// 일회성 스토리 키(매직 스트링 제거). 저장 데이터의 name 값과 일치해야 한다.
public static class SingleUseStoryKeys
{
    public const string Prologue = "Prologue";
    public const string InLobby = "In Lobby";
}
