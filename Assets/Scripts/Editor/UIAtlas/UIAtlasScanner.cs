using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public enum UIAtlasWarningKind
{
    NoTargetAtlas,          // 들어갈 아틀라스가 없음 (공용 아틀라스 미지정 등)
    BuiltinSprite,          // Unity 내장·패키지 스프라이트라 아틀라스에 넣을 수 없음
    UsedInWorld,            // UI와 월드(SpriteRenderer)에서 함께 쓰임
    Oversized,              // 기준보다 큰 스프라이트
    PackedInOtherAtlas,     // 툴이 관리하지 않는 다른 아틀라스에도 들어 있음
    InUnusedFolder,         // 미사용 폴더(Sprites_NonUsed 등)에 있는데 참조됨
    SharedAcrossAtlases,    // 여러 아틀라스에서 쓰여 공용 아틀라스로 보냄 (정보)
    Excluded,               // 제외 목록에 있어 아틀라스에 넣지 않음 (정보)
}

public class UIAtlasWarning
{
    public UIAtlasWarningKind kind;
    public string message;
    public List<UnityEngine.Object> relatedObjects = new List<UnityEngine.Object>();
}

public class UIAtlasPlan
{
    public SpriteAtlas atlas;
    public List<Sprite> sprites = new List<Sprite>();
    public long estimatedArea;
    public List<Sprite> addedSprites = new List<Sprite>();
    public List<string> removedSpriteKeys = new List<string>();
    public bool hasSnapshot;
}

public class UIAtlasScanResult
{
    public DateTime scannedTime;
    public bool includesScenes;
    public int scannedAssetCount;
    public List<SpriteUsage> usages = new List<SpriteUsage>();
    public Dictionary<Sprite, List<SpriteUsage>> usagesBySprite = new Dictionary<Sprite, List<SpriteUsage>>();
    public List<UIAtlasPlan> atlasPlans = new List<UIAtlasPlan>();
    public List<UIAtlasWarning> warnings = new List<UIAtlasWarning>();

    public UIAtlasPlan FindPlan(SpriteAtlas atlas)
    {
        return atlasPlans.FirstOrDefault(plan => plan.atlas == atlas);
    }
}

// 스캔 결과로 스프라이트마다 들어갈 아틀라스를 정하고(Scan), 실제 아틀라스에 반영한다(Apply).
public static class UIAtlasScanner
{
    private static readonly MethodInfo getPreviewTexturesMethod = typeof(SpriteAtlasExtensions).GetMethod(
        "GetPreviewTextures", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

    // 설정 규칙대로 사용처를 모으고, 스프라이트마다 들어갈 아틀라스와 경고를 정하는 함수. 아틀라스는 바꾸지 않는다.
    public static UIAtlasScanResult Scan(UIAtlasBuildConfig config, bool includeScenes, bool showProgress)
    {
        UISpriteUsageCollector collector = new UISpriteUsageCollector(config, includeScenes, showProgress);
        UIAtlasScanResult result = new UIAtlasScanResult
        {
            scannedTime = DateTime.Now,
            includesScenes = includeScenes && config.includeBuildScenes,
            usages = collector.Collect(),
        };
        result.scannedAssetCount = collector.ScannedAssetCount;

        foreach (SpriteUsage usage in result.usages)
        {
            if (result.usagesBySprite.TryGetValue(usage.sprite, out List<SpriteUsage> list) == false)
            {
                list = new List<SpriteUsage>();
                result.usagesBySprite.Add(usage.sprite, list);
            }
            list.Add(usage);
        }

        foreach (SpriteAtlas atlas in config.GetManagedAtlases())
        {
            result.atlasPlans.Add(new UIAtlasPlan { atlas = atlas });
        }

        AssignSprites(config, result);
        FindSpritesInOtherAtlases(config, result);
        CalculatePlanStatistics(config, result);
        return result;
    }

    // 스캔 결과대로 아틀라스의 패킹 목록을 교체하고 패킹한 뒤, 결과를 설정에 기록하는 함수
    // 패킹 중 Unity가 에셋을 다시 불러오면 기존 참조가 파괴될 수 있으므로, 패킹 후에는 경로로 다시 불러와 기록한다.
    // 기록이 끝난 설정 에셋을 반환한다. (호출한 쪽은 이 반환값으로 참조를 교체할 것)
    public static UIAtlasBuildConfig Apply(UIAtlasBuildConfig config, UIAtlasScanResult result)
    {
        string configPath = AssetDatabase.GetAssetPath(config);
        List<SpriteAtlas> managedAtlases = config.GetManagedAtlases();
        List<string> atlasPaths = managedAtlases.Select(AssetDatabase.GetAssetPath).ToList();
        List<List<string>> spriteKeysPerAtlas = new List<List<string>>();
        List<long> areaPerAtlas = new List<long>();

        foreach (SpriteAtlas atlas in managedAtlases)
        {
            UnityEngine.Object[] oldPackables = atlas.GetPackables();
            if (oldPackables.Length > 0) atlas.Remove(oldPackables);

            UIAtlasPlan plan = result.FindPlan(atlas);
            if (plan != null && plan.sprites.Count > 0) atlas.Add(plan.sprites.ToArray<UnityEngine.Object>());
            EditorUtility.SetDirty(atlas);

            spriteKeysPerAtlas.Add(plan != null ? plan.sprites.Select(GetSpriteKey).ToList() : new List<string>());
            areaPerAtlas.Add(plan != null ? plan.estimatedArea : 0);
        }
        AssetDatabase.SaveAssets();

        SpriteAtlasUtility.PackAtlases(managedAtlases.ToArray(), EditorUserBuildSettings.activeBuildTarget);

        config = AssetDatabase.LoadAssetAtPath<UIAtlasBuildConfig>(configPath);
        config.lastAppliedSnapshots.Clear();
        for (int index = 0; index < atlasPaths.Count; index++)
        {
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPaths[index]);
            config.lastAppliedSnapshots.Add(new UIAtlasBuildConfig.AtlasSnapshot
            {
                atlas = atlas,
                spriteKeys = spriteKeysPerAtlas[index],
                estimatedArea = areaPerAtlas[index],
                pageSizes = GetPageSizes(atlas),
            });
        }
        config.lastAppliedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        config.lastAppliedSceneTexturePaths = CollectBuildSceneTexturePaths().OrderBy(path => path, StringComparer.Ordinal).ToList();
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        return config;
    }

    // 스프라이트를 프로젝트 안에서 고유하게 가리키는 키(GUID:LocalFileId)를 만드는 함수
    public static string GetSpriteKey(Sprite sprite)
    {
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long localId))
        {
            return guid + ":" + localId;
        }
        return string.Empty;
    }

    // 키로 스프라이트를 찾아 "경로 (이름)" 형태로 설명하는 함수. 삭제된 스프라이트면 키를 그대로 보여준다.
    public static string DescribeSpriteKey(string spriteKey)
    {
        string[] parts = spriteKey.Split(':');
        if (parts.Length != 2) return spriteKey;
        string path = AssetDatabase.GUIDToAssetPath(parts[0]);
        if (string.IsNullOrEmpty(path)) return spriteKey + " (삭제된 에셋)";

        foreach (Sprite sprite in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>())
        {
            if (GetSpriteKey(sprite) == spriteKey) return path + " (" + sprite.name + ")";
        }
        return path + " (삭제된 스프라이트 " + parts[1] + ")";
    }

    // 패킹된 아틀라스 페이지 크기 목록을 반환하는 함수. 에디터 내부 API를 리플렉션으로 호출한다.
    public static List<string> GetPageSizes(SpriteAtlas atlas)
    {
        List<string> pageSizes = new List<string>();
        if (getPreviewTexturesMethod == null) return pageSizes;
        if (getPreviewTexturesMethod.Invoke(null, new object[] { atlas }) is Texture2D[] textures)
        {
            foreach (Texture2D texture in textures)
            {
                if (texture != null) pageSizes.Add(texture.width + "x" + texture.height);
            }
        }
        return pageSizes;
    }

    // 활성화된 빌드 씬이 (간접적으로) 의존하는 텍스처 경로를 모으는 함수. 빌드 전 검증에서 씬 변화 감지에 쓴다.
    public static HashSet<string> CollectBuildSceneTexturePaths()
    {
        HashSet<string> texturePaths = new HashSet<string>();
        string[] scenePaths = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        foreach (string dependencyPath in AssetDatabase.GetDependencies(scenePaths, true))
        {
            if (AssetDatabase.GetMainAssetTypeAtPath(dependencyPath) == typeof(Texture2D)) texturePaths.Add(dependencyPath);
        }
        return texturePaths;
    }

    // 스프라이트마다 들어갈 아틀라스를 정하는 함수
    // UI 사용처가 가리키는 아틀라스가 하나면 그 아틀라스, 여러 개면 공용 아틀라스로 보낸다.
    private static void AssignSprites(UIAtlasBuildConfig config, UIAtlasScanResult result)
    {
        HashSet<Sprite> excludedSprites = new HashSet<Sprite>();
        foreach (UnityEngine.Object excluded in config.excludedSprites)
        {
            excludedSprites.UnionWith(UISpriteUsageCollector.ExpandSprites(excluded));
        }

        UIAtlasWarning noTarget = CreateWarning(UIAtlasWarningKind.NoTargetAtlas, "들어갈 아틀라스가 없음 (공용 아틀라스를 지정하세요)");
        UIAtlasWarning builtin = CreateWarning(UIAtlasWarningKind.BuiltinSprite, "Unity 내장·패키지 스프라이트라 아틀라스에 넣을 수 없음: 이 Image에서 배치가 끊기므로 프로젝트 스프라이트로 교체를 검토");
        UIAtlasWarning usedInWorld = CreateWarning(UIAtlasWarningKind.UsedInWorld, "UI와 월드(SpriteRenderer)에서 함께 쓰임: UI 아틀라스에 들어가므로 월드에서도 이 아틀라스를 불러옴");
        UIAtlasWarning oversized = CreateWarning(UIAtlasWarningKind.Oversized, "가로나 세로가 " + config.oversizedSpriteSize + "px보다 큼: 축소하거나 제외 목록에 추가하는 것을 검토");
        UIAtlasWarning inUnusedFolder = CreateWarning(UIAtlasWarningKind.InUnusedFolder, "미사용 폴더에 있지만 참조되고 있음");
        UIAtlasWarning shared = CreateWarning(UIAtlasWarningKind.SharedAcrossAtlases, "여러 아틀라스에서 쓰여 공용 아틀라스로 보냄");
        UIAtlasWarning excludedWarning = CreateWarning(UIAtlasWarningKind.Excluded, "제외 목록에 있어 아틀라스에 넣지 않음");

        foreach (KeyValuePair<Sprite, List<SpriteUsage>> entry in result.usagesBySprite)
        {
            Sprite sprite = entry.Key;
            List<SpriteUsage> uiUsages = entry.Value.Where(usage => usage.IsWorldUsage == false).ToList();
            if (uiUsages.Count == 0) continue;   // 월드에서만 쓰이는 스프라이트는 대상이 아님

            if (AssetDatabase.GetAssetPath(sprite).StartsWith("Assets/", StringComparison.Ordinal) == false)
            {
                builtin.relatedObjects.Add(sprite);
                continue;
            }

            if (excludedSprites.Contains(sprite))
            {
                excludedWarning.relatedObjects.Add(sprite);
                continue;
            }

            List<SpriteAtlas> candidateAtlases = uiUsages.Select(usage => usage.targetAtlas).Where(atlas => atlas != null).Distinct().ToList();
            SpriteAtlas targetAtlas = candidateAtlases.Count == 1 ? candidateAtlases[0] : config.sharedAtlas;
            if (candidateAtlases.Count > 1) shared.relatedObjects.Add(sprite);

            UIAtlasPlan plan = targetAtlas != null ? result.FindPlan(targetAtlas) : null;
            if (plan == null)
            {
                noTarget.relatedObjects.Add(sprite);
                continue;
            }
            plan.sprites.Add(sprite);

            if (entry.Value.Any(usage => usage.IsWorldUsage)) usedInWorld.relatedObjects.Add(sprite);
            if (sprite.rect.width > config.oversizedSpriteSize || sprite.rect.height > config.oversizedSpriteSize) oversized.relatedObjects.Add(sprite);
            if (config.IsInUnusedSpriteFolder(AssetDatabase.GetAssetPath(sprite))) inUnusedFolder.relatedObjects.Add(sprite);
        }

        foreach (UIAtlasPlan plan in result.atlasPlans)
        {
            plan.sprites = plan.sprites
                .OrderBy(sprite => AssetDatabase.GetAssetPath(sprite), StringComparer.Ordinal)
                .ThenBy(sprite => sprite.name, StringComparer.Ordinal)
                .ToList();
        }

        AddWarningIfAny(result, noTarget);
        AddWarningIfAny(result, builtin);
        AddWarningIfAny(result, usedInWorld);
        AddWarningIfAny(result, oversized);
        AddWarningIfAny(result, inUnusedFolder);
        AddWarningIfAny(result, shared);
        AddWarningIfAny(result, excludedWarning);
    }

    // 툴이 관리하지 않는 다른 아틀라스에 같은 스프라이트가 들어 있는지 찾는 함수 (한 스프라이트는 아틀라스 하나에만 있어야 한다)
    private static void FindSpritesInOtherAtlases(UIAtlasBuildConfig config, UIAtlasScanResult result)
    {
        HashSet<Sprite> plannedSprites = new HashSet<Sprite>(result.atlasPlans.SelectMany(plan => plan.sprites));
        List<SpriteAtlas> managedAtlases = config.GetManagedAtlases();

        foreach (string guid in AssetDatabase.FindAssets("t:SpriteAtlas"))
        {
            SpriteAtlas otherAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AssetDatabase.GUIDToAssetPath(guid));
            if (otherAtlas == null || managedAtlases.Contains(otherAtlas)) continue;

            UIAtlasWarning warning = CreateWarning(UIAtlasWarningKind.PackedInOtherAtlas, "다른 아틀라스 '" + otherAtlas.name + "'에도 들어 있음");
            foreach (UnityEngine.Object packable in otherAtlas.GetPackables())
            {
                foreach (Sprite sprite in UISpriteUsageCollector.ExpandSprites(packable))
                {
                    if (plannedSprites.Contains(sprite)) warning.relatedObjects.Add(sprite);
                }
            }
            if (warning.relatedObjects.Count > 0) warning.relatedObjects.Insert(0, otherAtlas);
            AddWarningIfAny(result, warning);
        }
    }

    // 아틀라스별 예상 면적과 마지막 Apply 대비 추가·제거 목록을 계산하는 함수
    private static void CalculatePlanStatistics(UIAtlasBuildConfig config, UIAtlasScanResult result)
    {
        foreach (UIAtlasPlan plan in result.atlasPlans)
        {
            int padding = plan.atlas.GetPackingSettings().padding;
            plan.estimatedArea = 0;
            foreach (Sprite sprite in plan.sprites)
            {
                plan.estimatedArea += (long)(sprite.rect.width + padding) * (long)(sprite.rect.height + padding);
            }

            UIAtlasBuildConfig.AtlasSnapshot snapshot = config.FindSnapshot(plan.atlas);
            plan.hasSnapshot = snapshot != null;
            HashSet<string> previousKeys = snapshot != null ? new HashSet<string>(snapshot.spriteKeys) : new HashSet<string>();
            HashSet<string> currentKeys = new HashSet<string>();
            foreach (Sprite sprite in plan.sprites)
            {
                string key = GetSpriteKey(sprite);
                currentKeys.Add(key);
                if (previousKeys.Contains(key) == false) plan.addedSprites.Add(sprite);
            }
            plan.removedSpriteKeys = previousKeys.Where(key => currentKeys.Contains(key) == false).ToList();
        }
    }

    private static UIAtlasWarning CreateWarning(UIAtlasWarningKind kind, string message)
    {
        return new UIAtlasWarning { kind = kind, message = message };
    }

    private static void AddWarningIfAny(UIAtlasScanResult result, UIAtlasWarning warning)
    {
        if (warning.relatedObjects.Count > 0) result.warnings.Add(warning);
    }
}