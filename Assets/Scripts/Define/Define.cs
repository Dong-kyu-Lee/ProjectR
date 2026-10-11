using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

// 여러 클래스에서 사용되는 정의들을 통합한 기준 클래스
// 캐릭터, 적, 던전 종류 등의 전역적으로 쓰이는 "종류"에 대한 정의가 있는 클래스
// 단일 클래스에서만 사용하는 정의는 이 클래스에 포함시키지 않는다.
public static class Define
{
#region Enemy
    public enum NormalEnemyType
    {
        MeleeEnemy, RangedEnemy, MeleeDebuffEnemy
    }
    public enum MiddleBossEnemyType
    {
        Queen, 
    }
    public enum FinalBossEnemyType
    {
        Hero, 
    }
#endregion

    // 게임 씬 종류
    public enum SceneType
    {
        StartScene, LobbyScene, Normal, MiddleBoss, Shop, FinalBossScene, TestScene, StoryScene, EndScene,
    }

    public enum CharacterType
    {
        Bartender,
        Blacksmith,
    }

}
