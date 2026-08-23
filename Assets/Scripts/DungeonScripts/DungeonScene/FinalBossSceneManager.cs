// 최종보스 씬 매니저. 공통 흐름은 BossStorySceneManager가 가지며, 여기서는 차이점만 지정한다.
public class FinalBossSceneManager : BossStorySceneManager
{
    protected override StoryID TargetStoryID => StoryID.Temp_Final_Boss;
    protected override string BgmPath => "Sounds/BGM/FinalBossBGM";
    protected override float DeadDelay => 2.5f;
}
