using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

// 스프라이트가 어디서 어떻게 쓰이는지 구분
public enum SpriteUsageKind
{
    UIImage,            // Image.sprite
    UISpriteSwap,       // Selectable의 상태별 스프라이트 (Transition이 Sprite Swap일 때)
    UIAnimation,        // UI Animator 클립의 Image.m_Sprite 키
    UIComponentField,   // UI 오브젝트에 붙은 스크립트의 Sprite 필드 (런타임에 대입하는 용도)
    DataAsset,          // ScriptableObject의 Sprite 필드
    ManualInclude,      // 설정의 수동 포함 목록
    WorldRenderer,      // SpriteRenderer.sprite (아틀라스 대상 아님, 경고용)
    WorldAnimation,     // SpriteRenderer 애니메이션 클립 (아틀라스 대상 아님, 경고용)
}

public class SpriteUsage
{
    public Sprite sprite;
    public SpriteUsageKind kind;
    public SpriteAtlas targetAtlas;
    public string sourcePath;
    public string detail;

    public bool IsWorldUsage => kind == SpriteUsageKind.WorldRenderer || kind == SpriteUsageKind.WorldAnimation;
}

// 설정 규칙을 시작점으로 프리팹·씬·데이터 에셋을 따라가며 스프라이트 사용처를 모은다.
// - 컴포넌트 종류로 UI 사용과 월드 사용을 구분한다. (Image/Selectable/UI Animator vs SpriteRenderer)
// - 스크립트 필드가 참조하는 프리팹은 따라간다. 런타임에 Instantiate되는 슬롯 프리팹 등을 찾기 위함
// - 스크립트 필드가 참조하는 ScriptableObject와 Sprite는 UI 오브젝트(RectTransform)에서 참조할 때만 UI 사용으로 본다.
public class UISpriteUsageCollector
{
    private struct PendingAsset
    {
        public string path;
        public SpriteAtlas atlas;
    }

    private readonly UIAtlasBuildConfig config;
    private readonly bool includeScenes;
    private readonly bool showProgress;
    private readonly List<SpriteUsage> usages = new List<SpriteUsage>();
    private readonly HashSet<string> usageKeys = new HashSet<string>();
    private readonly Dictionary<string, SpriteAtlas> rootAssignments = new Dictionary<string, SpriteAtlas>();
    private readonly HashSet<string> visitedAssets = new HashSet<string>();
    private readonly Queue<PendingAsset> pendingAssets = new Queue<PendingAsset>();

    public int ScannedAssetCount { get; private set; }

    public UISpriteUsageCollector(UIAtlasBuildConfig config, bool includeScenes, bool showProgress)
    {
        this.config = config;
        this.includeScenes = includeScenes;
        this.showProgress = showProgress;
    }

    // 모든 규칙을 실행해 스프라이트 사용처 목록을 반환하는 함수
    public List<SpriteUsage> Collect()
    {
        BuildRootAssignments();
        foreach (KeyValuePair<string, SpriteAtlas> root in rootAssignments)
        {
            if (root.Value != null) EnqueueAsset(root.Key, root.Value, true);
        }

        try
        {
            while (pendingAssets.Count > 0)
            {
                PendingAsset pending = pendingAssets.Dequeue();
                ScannedAssetCount++;
                if (showProgress)
                {
                    float progress = (float)ScannedAssetCount / (ScannedAssetCount + pendingAssets.Count);
                    EditorUtility.DisplayProgressBar("UI Atlas Scan", pending.path, progress);
                }
                ScanAsset(pending.path, pending.atlas);
            }
        }
        finally
        {
            if (showProgress) EditorUtility.ClearProgressBar();
        }

        CollectManualSprites();
        return usages;
    }

    // Sprite, Texture2D(하위 스프라이트 전체), 폴더(안의 스프라이트 전체)를 Sprite 목록으로 펼치는 함수
    public static List<Sprite> ExpandSprites(UnityEngine.Object source)
    {
        List<Sprite> sprites = new List<Sprite>();
        if (source == null) return sprites;

        if (source is Sprite sprite)
        {
            sprites.Add(sprite);
            return sprites;
        }

        string path = AssetDatabase.GetAssetPath(source);
        if (string.IsNullOrEmpty(path)) return sprites;

        if (AssetDatabase.IsValidFolder(path))
        {
            HashSet<string> texturePaths = new HashSet<string>();
            foreach (string guid in AssetDatabase.FindAssets("t:Sprite", new[] { path }))
            {
                texturePaths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }
            foreach (string texturePath in texturePaths)
            {
                sprites.AddRange(AssetDatabase.LoadAllAssetsAtPath(texturePath).OfType<Sprite>());
            }
            return sprites;
        }

        sprites.AddRange(AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>());
        return sprites;
    }

    // 규칙별 시작점 에셋을 찾아 아틀라스를 정하는 함수. 우선순위가 높은 규칙부터 등록하고, 먼저 등록된 것을 유지한다.
    private void BuildRootAssignments()
    {
        foreach (UIAtlasBuildConfig.AssetRule rule in config.assetRules)
        {
            if (rule.asset == null) continue;
            TryAddRoot(AssetDatabase.GetAssetPath(rule.asset), rule.targetAtlas);
        }

        foreach (UIAtlasBuildConfig.DataTypeRule rule in config.dataTypeRules)
        {
            Type dataType = rule.dataType != null ? rule.dataType.GetClass() : null;
            if (dataType == null) continue;
            foreach (string guid in AssetDatabase.FindAssets("t:" + dataType.Name))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Type mainType = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (mainType != null && dataType.IsAssignableFrom(mainType)) TryAddRoot(path, rule.targetAtlas);
            }
        }

        foreach (UIAtlasBuildConfig.FolderRule rule in config.folderRules)
        {
            if (rule.folder == null) continue;
            string folderPath = AssetDatabase.GetAssetPath(rule.folder);
            if (AssetDatabase.IsValidFolder(folderPath) == false) continue;
            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { folderPath }))
            {
                TryAddRoot(AssetDatabase.GUIDToAssetPath(guid), rule.targetAtlas);
            }
        }

        if (includeScenes && config.includeBuildScenes)
        {
            foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
            {
                if (buildScene.enabled) TryAddRoot(buildScene.path, config.buildSceneAtlas);
            }
        }
    }

    private void TryAddRoot(string path, SpriteAtlas atlas)
    {
        if (string.IsNullOrEmpty(path) || config.IsExcludedPath(path)) return;
        if (rootAssignments.ContainsKey(path)) return;
        rootAssignments.Add(path, atlas);
    }

    // 에셋을 스캔 대기열에 넣는 함수. 명시적 규칙이 있는 에셋은 참조를 따라온 경우 무시한다(자기 규칙으로 따로 스캔됨).
    private void EnqueueAsset(string path, SpriteAtlas atlas, bool isRoot)
    {
        if (string.IsNullOrEmpty(path) || config.IsExcludedPath(path)) return;
        if (isRoot == false && rootAssignments.ContainsKey(path)) return;

        string visitKey = path + "|" + atlas.GetInstanceID();
        if (visitedAssets.Add(visitKey) == false) return;
        pendingAssets.Enqueue(new PendingAsset { path = path, atlas = atlas });
    }

    private void ScanAsset(string path, SpriteAtlas atlas)
    {
        if (path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
        {
            if (includeScenes) ScanScene(path, atlas);
            return;
        }

        if (path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) ScanHierarchy(prefab, path, atlas);
            return;
        }

        if (AssetDatabase.LoadMainAssetAtPath(path) is ScriptableObject data)
        {
            ScanSerializedReferences(data, true, SpriteUsageKind.DataAsset, atlas, path, data.name);
        }
    }

    // 씬을 추가 모드로 열어 스캔한 뒤, 이 함수가 연 씬이면 다시 닫는 함수
    private void ScanScene(string path, SpriteAtlas atlas)
    {
        Scene scene = SceneManager.GetSceneByPath(path);
        bool wasInHierarchy = scene.IsValid();
        bool wasLoaded = wasInHierarchy && scene.isLoaded;
        if (wasLoaded == false) scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

        try
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                ScanHierarchy(rootObject, path, atlas);
            }
        }
        finally
        {
            if (wasLoaded == false) EditorSceneManager.CloseScene(scene, wasInHierarchy == false);
        }
    }

    // 게임 오브젝트 계층의 컴포넌트를 훑어 스프라이트 사용처와 따라갈 에셋을 찾는 함수
    private void ScanHierarchy(GameObject root, string sourcePath, SpriteAtlas atlas)
    {
        foreach (Component component in root.GetComponentsInChildren<Component>(true))
        {
            if (component == null) continue;   // Missing Script

            bool isUIObject = component.transform is RectTransform;
            string hierarchyPath = GetHierarchyPath(component.transform);

            switch (component)
            {
                case Image image:
                    AddUsage(image.sprite, SpriteUsageKind.UIImage, atlas, sourcePath, hierarchyPath);
                    break;
                case Selectable selectable:
                    CollectSpriteSwap(selectable, atlas, sourcePath, hierarchyPath);
                    break;
                case SpriteRenderer spriteRenderer:
                    AddUsage(spriteRenderer.sprite, SpriteUsageKind.WorldRenderer, null, sourcePath, hierarchyPath);
                    break;
                case Animator animator:
                    CollectAnimatorClips(animator, atlas, sourcePath, hierarchyPath);
                    break;
            }

            if (component is MonoBehaviour behaviour && IsProjectScript(behaviour))
            {
                ScanSerializedReferences(behaviour, isUIObject, SpriteUsageKind.UIComponentField, atlas, sourcePath, hierarchyPath);
            }
        }
    }

    private void CollectSpriteSwap(Selectable selectable, SpriteAtlas atlas, string sourcePath, string hierarchyPath)
    {
        if (selectable.transition != Selectable.Transition.SpriteSwap) return;
        SpriteState state = selectable.spriteState;
        AddUsage(state.highlightedSprite, SpriteUsageKind.UISpriteSwap, atlas, sourcePath, hierarchyPath + " (highlighted)");
        AddUsage(state.pressedSprite, SpriteUsageKind.UISpriteSwap, atlas, sourcePath, hierarchyPath + " (pressed)");
        AddUsage(state.selectedSprite, SpriteUsageKind.UISpriteSwap, atlas, sourcePath, hierarchyPath + " (selected)");
        AddUsage(state.disabledSprite, SpriteUsageKind.UISpriteSwap, atlas, sourcePath, hierarchyPath + " (disabled)");
    }

    // Animator 컨트롤러의 클립에서 스프라이트 키프레임을 찾는 함수. Image 대상이면 UI, SpriteRenderer 대상이면 월드로 기록한다.
    private void CollectAnimatorClips(Animator animator, SpriteAtlas atlas, string sourcePath, string hierarchyPath)
    {
        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        if (controller == null) return;

        foreach (AnimationClip clip in controller.animationClips.Distinct())
        {
            if (clip == null) continue;
            foreach (EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                if (binding.propertyName != "m_Sprite") continue;
                bool isImageBinding = typeof(Image).IsAssignableFrom(binding.type);
                bool isRendererBinding = typeof(SpriteRenderer).IsAssignableFrom(binding.type);
                if (isImageBinding == false && isRendererBinding == false) continue;

                SpriteUsageKind kind = isImageBinding ? SpriteUsageKind.UIAnimation : SpriteUsageKind.WorldAnimation;
                SpriteAtlas targetAtlas = isImageBinding ? atlas : null;
                string detail = hierarchyPath + " > " + clip.name;
                foreach (ObjectReferenceKeyframe keyframe in AnimationUtility.GetObjectReferenceCurve(clip, binding))
                {
                    AddUsage(keyframe.value as Sprite, kind, targetAtlas, sourcePath, detail);
                }
            }
        }
    }

    // 직렬화된 필드를 훑어 Sprite 참조를 기록하고, 참조된 프리팹·ScriptableObject를 대기열에 넣는 함수
    private void ScanSerializedReferences(UnityEngine.Object target, bool isUIContext, SpriteUsageKind spriteKind, SpriteAtlas atlas, string sourcePath, string detail)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.GetIterator();
        while (property.Next(true))
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
            UnityEngine.Object value = property.objectReferenceValue;
            if (value == null) continue;

            if (value is Sprite sprite)
            {
                if (isUIContext) AddUsage(sprite, spriteKind, atlas, sourcePath, detail + " > " + property.propertyPath);
                continue;
            }

            string referencedPath = AssetDatabase.GetAssetPath(value);
            if (string.IsNullOrEmpty(referencedPath) || referencedPath == sourcePath) continue;

            if (value is GameObject || value is Component)
            {
                if (referencedPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)) EnqueueAsset(referencedPath, atlas, false);
            }
            else if (value is ScriptableObject && isUIContext)
            {
                EnqueueAsset(referencedPath, atlas, false);
            }
        }
    }

    private void CollectManualSprites()
    {
        foreach (UIAtlasBuildConfig.ManualSpriteRule rule in config.manualSprites)
        {
            if (rule.sprite == null || rule.targetAtlas == null) continue;
            string sourcePath = AssetDatabase.GetAssetPath(rule.sprite);
            foreach (Sprite sprite in ExpandSprites(rule.sprite))
            {
                AddUsage(sprite, SpriteUsageKind.ManualInclude, rule.targetAtlas, sourcePath, "수동 포함");
            }
        }
    }

    private void AddUsage(Sprite sprite, SpriteUsageKind kind, SpriteAtlas atlas, string sourcePath, string detail)
    {
        if (sprite == null) return;
        int atlasId = atlas != null ? atlas.GetInstanceID() : 0;
        string usageKey = sprite.GetInstanceID() + "|" + (int)kind + "|" + atlasId + "|" + sourcePath + "|" + detail;
        if (usageKeys.Add(usageKey) == false) return;

        usages.Add(new SpriteUsage
        {
            sprite = sprite,
            kind = kind,
            targetAtlas = atlas,
            sourcePath = sourcePath,
            detail = detail,
        });
    }

    // 플러그인·엔진 컴포넌트가 아닌 프로젝트 스크립트인지 확인하는 함수 (asmdef를 쓰지 않으므로 Assembly-CSharp 계열)
    private static bool IsProjectScript(MonoBehaviour behaviour)
    {
        string assemblyName = behaviour.GetType().Assembly.GetName().Name;
        return assemblyName.StartsWith("Assembly-CSharp", StringComparison.Ordinal);
    }

    private static string GetHierarchyPath(Transform transform)
    {
        string path = transform.name;
        Transform parent = transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
}