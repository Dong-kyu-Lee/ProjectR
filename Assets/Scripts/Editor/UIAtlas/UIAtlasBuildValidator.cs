using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// 빌드 직전에 UI 아틀라스 구성이 최신인지 확인하고, 아니면 콘솔에 경고만 남긴다.
// 빌드를 막거나 자동으로 Apply하지 않는다. 빌드 중에 씬을 여닫지 않도록 씬은 텍스처 의존성 변화로만 확인한다.
public class UIAtlasBuildValidator : IPreprocessBuildWithReport
{
    private const int MaxListedCount = 20;

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:" + nameof(UIAtlasBuildConfig)))
        {
            UIAtlasBuildConfig config = AssetDatabase.LoadAssetAtPath<UIAtlasBuildConfig>(AssetDatabase.GUIDToAssetPath(guid));
            if (config == null || config.lastAppliedSnapshots.Count == 0) continue;
            ValidateConfig(config);
        }
    }

    // 프리팹·데이터·수동 규칙은 다시 스캔해 마지막 Apply에 없는 스프라이트를 찾고, 빌드 씬은 새로 생긴 텍스처 의존성을 찾는 함수
    private static void ValidateConfig(UIAtlasBuildConfig config)
    {
        UIAtlasScanResult result = UIAtlasScanner.Scan(config, false, false);
        HashSet<string> appliedKeys = new HashSet<string>(config.lastAppliedSnapshots.SelectMany(snapshot => snapshot.spriteKeys));
        List<Sprite> missingSprites = result.atlasPlans
            .SelectMany(plan => plan.sprites)
            .Where(sprite => appliedKeys.Contains(UIAtlasScanner.GetSpriteKey(sprite)) == false)
            .ToList();

        HashSet<string> appliedSceneTextures = new HashSet<string>(config.lastAppliedSceneTexturePaths);
        List<string> newSceneTextures = UIAtlasScanner.CollectBuildSceneTexturePaths()
            .Where(path => appliedSceneTextures.Contains(path) == false)
            .OrderBy(path => path)
            .ToList();

        if (missingSprites.Count == 0 && newSceneTextures.Count == 0) return;

        StringBuilder message = new StringBuilder();
        message.AppendLine("[UI Atlas] 마지막 Apply(" + config.lastAppliedTime + ") 이후 UI 구성이 바뀌었을 수 있습니다. Tools > UI Atlas Builder에서 Scan 후 Apply를 확인하세요. (빌드는 계속 진행됩니다)");
        if (missingSprites.Count > 0)
        {
            message.AppendLine("- 아틀라스에 없는 UI 스프라이트 " + missingSprites.Count + "개:");
            foreach (Sprite sprite in missingSprites.Take(MaxListedCount))
            {
                message.AppendLine("    " + AssetDatabase.GetAssetPath(sprite) + " (" + sprite.name + ")");
            }
        }
        if (newSceneTextures.Count > 0)
        {
            message.AppendLine("- 빌드 씬이 새로 참조하는 텍스처 " + newSceneTextures.Count + "개 (UI용이 아니면 무시해도 됩니다):");
            foreach (string path in newSceneTextures.Take(MaxListedCount))
            {
                message.AppendLine("    " + path);
            }
        }
        Debug.LogWarning(message.ToString(), config);
    }
}