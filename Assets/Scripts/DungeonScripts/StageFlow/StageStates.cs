using UnityEngine.SceneManagement;

// 스테이지 흐름 그래프의 노드 하나에 진입/이탈할 때의 동작을 정의하는 상태
public interface IStageState
{
    void Enter(Stage stage, StageNode node);
    void Exit(Stage stage);
}

public static class StageStateFactory
{
    public static IStageState Create(StageNodeType type)
    {
        switch (type)
        {
            case StageNodeType.MiddleBoss:
            case StageNodeType.FinalBoss:
                return new BossState();
            case StageNodeType.Shop:
                return new ShopState();
            default:
                return new NormalDungeonState();
        }
    }
}

// 일반 던전: 이미 던전 씬이면 그 자리에서 던전을 다시 생성하고, 아니면 던전 씬을 로드한 뒤 생성한다.
public class NormalDungeonState : IStageState
{
    public void Enter(Stage stage, StageNode node)
    {
        if (SceneManager.GetActiveScene().name == node.SceneName)
            stage.CreateDungeon(node);
        else
            stage.LoadSceneThen(node.SceneType, node.SceneName, () => stage.CreateDungeon(node), true);
    }

    public void Exit(Stage stage)
    {
        stage.RemoveDungeon();
    }
}

// 보스: 스테이지마다 컷씬이 재생되도록 진입 시 스토리를 Available로 되돌린 뒤 보스 씬으로 이동한다.
// 보스 처치 여부는 Stage.IsCurrentNodeCleared로 관리한다. (컷씬 복귀 시 씬이 다시 로드되어도 유지됨)
public class BossState : IStageState
{
    public void Enter(Stage stage, StageNode node)
    {
        if (node.playStory)
            StorySystem.Instance.SetStoryState(node.storyID, StoryState.Available);
        stage.LoadSceneThen(node.SceneType, node.SceneName, null);
    }

    public void Exit(Stage stage) { }
}

// 상점: 상점 씬으로 이동한다. 씬 내부 동작은 ShopSceneManager가 처리한다.
public class ShopState : IStageState
{
    public void Enter(Stage stage, StageNode node)
    {
        stage.LoadSceneThen(node.SceneType, node.SceneName, null);
    }

    public void Exit(Stage stage) { }
}
