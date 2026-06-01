public static class SceneTypeExtensions
{
    // 인게임 UI가 필요한 던전 씬 (Normal, MiddleBoss, Shop, FinalBossScene)
    public static bool IsDungeonScene(this SceneType type) =>
        type == SceneType.Normal ||
        type == SceneType.MiddleBoss ||
        type == SceneType.Shop ||
        type == SceneType.FinalBossScene;

    // 게임 상태를 완전히 초기화하는 씬 (StartScene, LobbyScene)
    // 플레이어 오브젝트와 UI가 파괴되고, 각종 상태가 리셋됨
    public static bool IsReturnScene(this SceneType type) =>
        type == SceneType.StartScene ||
        type == SceneType.LobbyScene;

    // 플레이어 오브젝트가 비활성화되는 씬 (파괴 아님)
    // 연출 또는 비전투 상황에서 플레이어를 화면에서 숨김
    public static bool IsPlayerDeactivatedScene(this SceneType type) =>
        type == SceneType.MiddleBoss ||
        type == SceneType.FinalBossScene ||
        type == SceneType.StoryScene ||
        type == SceneType.EndScene;

    // 인게임 UI가 비활성화(숨김)되는 씬 (파괴 아님)
    public static bool IsUIHiddenScene(this SceneType type) =>
        type == SceneType.StoryScene ||
        type == SceneType.EndScene;
}
