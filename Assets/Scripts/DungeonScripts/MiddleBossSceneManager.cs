// 중간보스 씬 매니저. 공통 흐름은 BossStorySceneManager가 가지며, 여기서는 차이점만 지정한다.
public class MiddleBossSceneManager : BossStorySceneManager
{
    protected override StoryID TargetStoryID => StoryID.Temp_Middle_Boss;
    protected override string BgmPath => "Sounds/BGM/MiddleBossBGM";
    protected override float DeadDelay => 1.5f;
}
