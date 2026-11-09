using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// 참조 기반 UI 아틀라스 구성 툴 (Tools > UI Atlas Builder)
// Scan: 규칙대로 사용처를 모아 아틀라스별 구성과 경고를 보여준다. 에셋은 바꾸지 않는다.
// Apply: 스캔 결과대로 아틀라스의 패킹 목록을 교체하고 패킹한다.
// Export Report: 스캔 결과를 Markdown·CSV로 저장한다.
public class UIAtlasBuilderWindow : EditorWindow
{
    private const int MaxSpritesPerList = 300;

    private UIAtlasBuildConfig config;
    private UIAtlasScanResult scanResult;
    private Vector2 scrollPosition;
    private readonly Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    [MenuItem("Tools/UI Atlas Builder")]
    private static void OpenWindow()
    {
        GetWindow<UIAtlasBuilderWindow>("UI Atlas Builder");
    }

    private void OnEnable()
    {
        if (config != null) return;
        string guid = AssetDatabase.FindAssets("t:" + nameof(UIAtlasBuildConfig)).FirstOrDefault();
        if (guid != null) config = AssetDatabase.LoadAssetAtPath<UIAtlasBuildConfig>(AssetDatabase.GUIDToAssetPath(guid));
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        config = (UIAtlasBuildConfig)EditorGUILayout.ObjectField("설정", config, typeof(UIAtlasBuildConfig), false);
        if (EditorGUI.EndChangeCheck()) scanResult = null;

        if (config == null)
        {
            EditorGUILayout.HelpBox("UIAtlasBuildConfig 에셋을 지정하세요. (Create > ProjectR > UI Atlas Build Config)", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField("마지막 Apply", string.IsNullOrEmpty(config.lastAppliedTime) ? "없음" : config.lastAppliedTime);

        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                if (GUILayout.Button("Scan", GUILayout.Height(28))) RunScan();
            }
            using (new EditorGUI.DisabledScope(scanResult == null || EditorApplication.isPlayingOrWillChangePlaymode))
            {
                if (GUILayout.Button("Apply", GUILayout.Height(28))) RunApply();
            }
            using (new EditorGUI.DisabledScope(scanResult == null))
            {
                if (GUILayout.Button("Export Report", GUILayout.Height(28))) RunExport();
            }
        }

        if (scanResult == null)
        {
            EditorGUILayout.HelpBox("Scan은 아틀라스를 바꾸지 않습니다. 결과를 확인한 뒤 Apply하세요.\n빌드 씬을 잠시 추가로 열었다가 닫습니다.", MessageType.None);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        DrawSummary();
        DrawWarnings();
        DrawAtlasDetails();
        EditorGUILayout.EndScrollView();
    }

    private void RunScan()
    {
        scanResult = UIAtlasScanner.Scan(config, true, true);
        foldoutStates.Clear();
        GUIUtility.ExitGUI();
    }

    // 변경 요약을 보여주고 확인을 받은 뒤 Apply하는 함수
    private void RunApply()
    {
        StringBuilder summary = new StringBuilder();
        foreach (UIAtlasPlan plan in scanResult.atlasPlans)
        {
            summary.AppendLine(plan.atlas.name + ": " + plan.sprites.Count + "개 (+" + plan.addedSprites.Count + " / -" + plan.removedSpriteKeys.Count + ")");
        }
        int warningCount = scanResult.warnings.Count(warning => IsInformation(warning.kind) == false);
        if (warningCount > 0) summary.AppendLine("\n확인하지 않은 경고 " + warningCount + "종류가 있습니다.");
        summary.AppendLine("\n아틀라스의 패킹 목록을 교체하고 다시 패킹합니다.");

        if (EditorUtility.DisplayDialog("UI Atlas Apply", summary.ToString(), "Apply", "취소") == false) return;

        try
        {
            EditorUtility.DisplayProgressBar("UI Atlas Apply", "아틀라스 패킹 중", 0.5f);
            config = UIAtlasScanner.Apply(config, scanResult);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        // 패킹 중 에셋이 다시 로드되어 이전 스캔 결과의 참조가 무효일 수 있으므로 다시 스캔한다.
        RunScan();
    }

    private void RunExport()
    {
        string folderPath = EditorUtility.SaveFolderPanel("리포트를 저장할 폴더", "", "");
        if (string.IsNullOrEmpty(folderPath)) return;
        string markdownPath = UIAtlasReportExporter.Export(config, scanResult, folderPath);
        EditorUtility.RevealInFinder(markdownPath);
        GUIUtility.ExitGUI();
    }

    private void DrawSummary()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("스캔 결과 (" + scanResult.scannedTime.ToString("HH:mm:ss") + ", 에셋 " + scanResult.scannedAssetCount + "개 스캔)", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            GUILayout.Label("아틀라스", GUILayout.Width(160));
            GUILayout.Label("스프라이트", GUILayout.Width(70));
            GUILayout.Label("예상 면적", GUILayout.Width(90));
            GUILayout.Label("추가 / 제거", GUILayout.Width(90));
            GUILayout.Label("마지막 Apply 페이지");
        }
        foreach (UIAtlasPlan plan in scanResult.atlasPlans)
        {
            UIAtlasBuildConfig.AtlasSnapshot snapshot = config.FindSnapshot(plan.atlas);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(plan.atlas.name, EditorStyles.linkLabel, GUILayout.Width(160))) PingObject(plan.atlas);
                GUILayout.Label(plan.sprites.Count.ToString(), GUILayout.Width(70));
                GUILayout.Label(plan.estimatedArea.ToString("N0"), GUILayout.Width(90));
                GUILayout.Label(plan.hasSnapshot ? "+" + plan.addedSprites.Count + " / -" + plan.removedSpriteKeys.Count : "첫 Apply", GUILayout.Width(90));
                GUILayout.Label(snapshot != null && snapshot.pageSizes.Count > 0 ? string.Join(", ", snapshot.pageSizes) : "-");
            }
        }
    }

    private void DrawWarnings()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("경고", EditorStyles.boldLabel);
        if (scanResult.warnings.Count == 0)
        {
            EditorGUILayout.HelpBox("경고 없음", MessageType.Info);
            return;
        }

        for (int index = 0; index < scanResult.warnings.Count; index++)
        {
            UIAtlasWarning warning = scanResult.warnings[index];
            List<Sprite> sprites = warning.relatedObjects.OfType<Sprite>().ToList();
            MessageType messageType = IsInformation(warning.kind) ? MessageType.Info : MessageType.Warning;
            EditorGUILayout.HelpBox(warning.kind + " (" + sprites.Count + "): " + warning.message, messageType);

            UnityEngine.Object relatedAtlas = warning.relatedObjects.FirstOrDefault(related => (related is Sprite) == false);
            if (relatedAtlas != null && GUILayout.Button(relatedAtlas.name, EditorStyles.linkLabel)) PingObject(relatedAtlas);

            if (DrawFoldout("warning" + index, "목록 보기"))
            {
                DrawSpriteList(sprites);
            }
        }
    }

    private void DrawAtlasDetails()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("아틀라스별 스프라이트", EditorStyles.boldLabel);
        foreach (UIAtlasPlan plan in scanResult.atlasPlans)
        {
            if (DrawFoldout("atlas" + plan.atlas.GetInstanceID(), plan.atlas.name + " (" + plan.sprites.Count + ")"))
            {
                if (plan.addedSprites.Count > 0 && DrawFoldout("added" + plan.atlas.GetInstanceID(), "    추가 (" + plan.addedSprites.Count + ")"))
                {
                    DrawSpriteList(plan.addedSprites);
                }
                if (plan.removedSpriteKeys.Count > 0 && DrawFoldout("removed" + plan.atlas.GetInstanceID(), "    제거 (" + plan.removedSpriteKeys.Count + ")"))
                {
                    foreach (string key in plan.removedSpriteKeys) EditorGUILayout.LabelField(UIAtlasScanner.DescribeSpriteKey(key), EditorStyles.miniLabel);
                }
                DrawSpriteList(plan.sprites);
            }
        }
    }

    // 스프라이트 목록을 그리는 함수. 이름을 누르면 에셋을 선택하고, 마우스를 올리면 사용처를 보여준다.
    private void DrawSpriteList(List<Sprite> sprites)
    {
        EditorGUI.indentLevel++;
        foreach (Sprite sprite in sprites.Take(MaxSpritesPerList))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUI.indentLevel * 15);
                GUIContent label = new GUIContent(sprite.name + "  (" + (int)sprite.rect.width + "x" + (int)sprite.rect.height + ")", GetUsageTooltip(sprite));
                if (GUILayout.Button(label, EditorStyles.linkLabel, GUILayout.Width(260))) PingObject(sprite);
                GUILayout.Label(AssetDatabase.GetAssetPath(sprite), EditorStyles.miniLabel);
            }
        }
        if (sprites.Count > MaxSpritesPerList)
        {
            EditorGUILayout.LabelField("... 외 " + (sprites.Count - MaxSpritesPerList) + "개 (전체 목록은 Export Report)", EditorStyles.miniLabel);
        }
        EditorGUI.indentLevel--;
    }

    private string GetUsageTooltip(Sprite sprite)
    {
        if (scanResult.usagesBySprite.TryGetValue(sprite, out List<SpriteUsage> usages) == false) return string.Empty;
        IEnumerable<string> lines = usages.Take(10).Select(usage => usage.kind + ": " + Path.GetFileName(usage.sourcePath) + " / " + usage.detail);
        string tooltip = string.Join("\n", lines);
        if (usages.Count > 10) tooltip += "\n... 외 " + (usages.Count - 10) + "곳";
        return tooltip;
    }

    private bool DrawFoldout(string key, string label)
    {
        foldoutStates.TryGetValue(key, out bool isOpen);
        isOpen = EditorGUILayout.Foldout(isOpen, label, true);
        foldoutStates[key] = isOpen;
        return isOpen;
    }

    private static bool IsInformation(UIAtlasWarningKind kind)
    {
        return kind == UIAtlasWarningKind.SharedAcrossAtlases || kind == UIAtlasWarningKind.Excluded;
    }

    private static void PingObject(UnityEngine.Object target)
    {
        Selection.activeObject = target;
        EditorGUIUtility.PingObject(target);
    }
}