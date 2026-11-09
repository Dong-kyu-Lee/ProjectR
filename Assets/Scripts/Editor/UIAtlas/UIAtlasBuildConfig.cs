using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

// UI 아틀라스 자동 구성 규칙
// 어떤 에셋에서 찾은 스프라이트를 어느 아틀라스에 넣을지 정의한다. UIAtlasBuilderWindow(Tools > UI Atlas Builder)에서 사용한다.
// 시작점 우선순위: 개별 에셋 규칙 > 데이터 타입 규칙 > 폴더 규칙 > 빌드 씬
// 명시적 규칙이 있는 에셋은 다른 에셋에서 참조되더라도 항상 자기 규칙의 아틀라스를 따른다.
// 여러 아틀라스에서 함께 쓰는 스프라이트는 공용 아틀라스로 보낸다.
[CreateAssetMenu(fileName = "UIAtlasBuildConfig", menuName = "ProjectR/UI Atlas Build Config")]
public class UIAtlasBuildConfig : ScriptableObject
{
    [Serializable]
    public class AssetRule
    {
        [Tooltip("프리팹, 씬, ScriptableObject")]
        public UnityEngine.Object asset;
        [Tooltip("비워두면 이 에셋은 스캔하지 않는다.")]
        public SpriteAtlas targetAtlas;
    }

    [Serializable]
    public class DataTypeRule
    {
        [Tooltip("ScriptableObject 스크립트. 하위 클래스 에셋도 포함된다.")]
        public MonoScript dataType;
        public SpriteAtlas targetAtlas;
    }

    [Serializable]
    public class FolderRule
    {
        [Tooltip("이 폴더 아래(하위 폴더 포함) 프리팹 전부를 시작점으로 삼는다.")]
        public DefaultAsset folder;
        public SpriteAtlas targetAtlas;
    }

    [Serializable]
    public class ManualSpriteRule
    {
        [Tooltip("Sprite 또는 Texture2D(하위 스프라이트 전체)")]
        public UnityEngine.Object sprite;
        public SpriteAtlas targetAtlas;
    }

    [Serializable]
    public class AtlasSnapshot
    {
        public SpriteAtlas atlas;
        public List<string> spriteKeys = new List<string>();
        public long estimatedArea;
        public List<string> pageSizes = new List<string>();
    }

    [Header("공용 아틀라스: 여러 아틀라스에서 함께 쓰는 스프라이트가 모이는 곳")]
    public SpriteAtlas sharedAtlas;

    [Header("빌드 씬 (EditorBuildSettings에서 활성화된 씬)")]
    public bool includeBuildScenes = true;
    public SpriteAtlas buildSceneAtlas;

    [Header("규칙")]
    [Tooltip("특정 프리팹·씬·ScriptableObject를 지정한 아틀라스로 보낸다. 다른 규칙보다 우선한다.")]
    public List<AssetRule> assetRules = new List<AssetRule>();
    [Tooltip("해당 타입의 ScriptableObject를 모두 찾아 그 안의 Sprite 필드를 아틀라스로 보낸다.")]
    public List<DataTypeRule> dataTypeRules = new List<DataTypeRule>();
    [Tooltip("폴더 아래 프리팹을 모두 시작점으로 삼는다.")]
    public List<FolderRule> folderRules = new List<FolderRule>();

    [Header("수동 포함 / 제외")]
    [Tooltip("코드에서만 대입해서 스캔으로 찾을 수 없는 스프라이트")]
    public List<ManualSpriteRule> manualSprites = new List<ManualSpriteRule>();
    [Tooltip("어느 아틀라스에도 넣지 않을 스프라이트. Sprite, Texture2D 또는 폴더")]
    public List<UnityEngine.Object> excludedSprites = new List<UnityEngine.Object>();
    [Tooltip("스캔하지 않을 폴더 (레거시·테스트 프리팹 등)")]
    public List<DefaultAsset> excludedFolders = new List<DefaultAsset>();

    [Header("경고 기준")]
    [Tooltip("가로나 세로가 이 값(px)보다 큰 스프라이트를 경고한다.")]
    public int oversizedSpriteSize = 256;
    [Tooltip("이 폴더의 스프라이트가 참조되면 경고한다. (예: Sprites_NonUsed)")]
    public List<DefaultAsset> unusedSpriteFolders = new List<DefaultAsset>();

    [Header("마지막 Apply 기록 (툴이 자동으로 기록한다)")]
    public string lastAppliedTime;
    public List<AtlasSnapshot> lastAppliedSnapshots = new List<AtlasSnapshot>();
    public List<string> lastAppliedSceneTexturePaths = new List<string>();

    // 규칙에 등장하는 모든 아틀라스(공용·빌드 씬 포함)를 중복 없이 반환하는 함수
    public List<SpriteAtlas> GetManagedAtlases()
    {
        List<SpriteAtlas> atlases = new List<SpriteAtlas>();
        AddAtlasIfNew(atlases, sharedAtlas);
        AddAtlasIfNew(atlases, buildSceneAtlas);
        foreach (AssetRule rule in assetRules) AddAtlasIfNew(atlases, rule.targetAtlas);
        foreach (DataTypeRule rule in dataTypeRules) AddAtlasIfNew(atlases, rule.targetAtlas);
        foreach (FolderRule rule in folderRules) AddAtlasIfNew(atlases, rule.targetAtlas);
        foreach (ManualSpriteRule rule in manualSprites) AddAtlasIfNew(atlases, rule.targetAtlas);
        return atlases;
    }

    // 에셋 경로가 제외 폴더 아래에 있는지 확인하는 함수
    public bool IsExcludedPath(string assetPath)
    {
        return IsUnderAnyFolder(assetPath, excludedFolders);
    }

    // 에셋 경로가 미사용 스프라이트 폴더 아래에 있는지 확인하는 함수
    public bool IsInUnusedSpriteFolder(string assetPath)
    {
        return IsUnderAnyFolder(assetPath, unusedSpriteFolders);
    }

    // 아틀라스의 마지막 Apply 기록을 찾는 함수. 기록이 없으면 null
    public AtlasSnapshot FindSnapshot(SpriteAtlas atlas)
    {
        foreach (AtlasSnapshot snapshot in lastAppliedSnapshots)
        {
            if (snapshot.atlas == atlas) return snapshot;
        }
        return null;
    }

    private static void AddAtlasIfNew(List<SpriteAtlas> atlases, SpriteAtlas atlas)
    {
        if (atlas != null && atlases.Contains(atlas) == false) atlases.Add(atlas);
    }

    private static bool IsUnderAnyFolder(string assetPath, List<DefaultAsset> folders)
    {
        if (string.IsNullOrEmpty(assetPath)) return false;
        foreach (DefaultAsset folder in folders)
        {
            if (folder == null) continue;
            string folderPath = AssetDatabase.GetAssetPath(folder);
            if (string.IsNullOrEmpty(folderPath)) continue;
            if (assetPath.StartsWith(folderPath + "/", StringComparison.Ordinal)) return true;
        }
        return false;
    }
}