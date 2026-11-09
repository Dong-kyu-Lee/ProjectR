using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// 스캔 결과를 Markdown 요약과 CSV 상세 목록으로 저장한다. 전후 비교·포트폴리오 자료용
public static class UIAtlasReportExporter
{
    // 스캔 결과를 지정한 폴더에 Markdown·CSV 파일로 저장하고, 저장한 Markdown 경로를 반환하는 함수
    public static string Export(UIAtlasBuildConfig config, UIAtlasScanResult result, string folderPath)
    {
        string fileName = "UIAtlasReport_" + result.scannedTime.ToString("yyyyMMdd_HHmm");
        string markdownPath = Path.Combine(folderPath, fileName + ".md");
        string csvPath = Path.Combine(folderPath, fileName + ".csv");

        File.WriteAllText(markdownPath, BuildMarkdown(config, result), new UTF8Encoding(false));
        File.WriteAllText(csvPath, BuildCsv(result), new UTF8Encoding(true));   // 엑셀에서 한글이 깨지지 않도록 BOM 포함
        return markdownPath;
    }

    private static string BuildMarkdown(UIAtlasBuildConfig config, UIAtlasScanResult result)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("# UI Atlas Report");
        builder.AppendLine();
        builder.AppendLine("- 스캔 시각: " + result.scannedTime.ToString("yyyy-MM-dd HH:mm"));
        builder.AppendLine("- 설정: " + AssetDatabase.GetAssetPath(config));
        builder.AppendLine("- 빌드 씬 포함: " + (result.includesScenes ? "예" : "아니오"));
        builder.AppendLine("- 스캔한 에셋 수: " + result.scannedAssetCount);
        builder.AppendLine("- UI에서 쓰이는 스프라이트: " + result.usagesBySprite.Count(entry => entry.Value.Any(usage => usage.IsWorldUsage == false)));
        builder.AppendLine("- 마지막 Apply: " + (string.IsNullOrEmpty(config.lastAppliedTime) ? "없음" : config.lastAppliedTime));
        builder.AppendLine();

        builder.AppendLine("## 아틀라스별 요약");
        builder.AppendLine();
        builder.AppendLine("| 아틀라스 | 스프라이트 수 | 예상 면적(px, padding 포함) | 마지막 Apply 페이지 | 추가 | 제거 |");
        builder.AppendLine("|---|---:|---:|---|---:|---:|");
        foreach (UIAtlasPlan plan in result.atlasPlans)
        {
            UIAtlasBuildConfig.AtlasSnapshot snapshot = config.FindSnapshot(plan.atlas);
            string pages = snapshot != null && snapshot.pageSizes.Count > 0 ? string.Join(", ", snapshot.pageSizes) : "-";
            string added = plan.hasSnapshot ? plan.addedSprites.Count.ToString() : "(첫 Apply)";
            builder.AppendLine("| " + plan.atlas.name + " | " + plan.sprites.Count + " | " + plan.estimatedArea.ToString("N0") + " | " + pages + " | " + added + " | " + plan.removedSpriteKeys.Count + " |");
        }
        builder.AppendLine();

        builder.AppendLine("## 경고");
        builder.AppendLine();
        if (result.warnings.Count == 0) builder.AppendLine("없음");
        foreach (UIAtlasWarning warning in result.warnings)
        {
            List<Sprite> sprites = warning.relatedObjects.OfType<Sprite>().ToList();
            builder.AppendLine("### " + warning.kind + " (" + sprites.Count + ")");
            builder.AppendLine();
            builder.AppendLine(warning.message);
            builder.AppendLine();
            foreach (Sprite sprite in sprites)
            {
                builder.AppendLine("- " + DescribeSprite(sprite) + " ← " + DescribeSources(result, sprite, 3));
            }
            builder.AppendLine();
        }

        builder.AppendLine("## 마지막 Apply 대비 변경");
        builder.AppendLine();
        foreach (UIAtlasPlan plan in result.atlasPlans)
        {
            if (plan.hasSnapshot == false || (plan.addedSprites.Count == 0 && plan.removedSpriteKeys.Count == 0)) continue;
            builder.AppendLine("### " + plan.atlas.name);
            builder.AppendLine();
            foreach (Sprite sprite in plan.addedSprites) builder.AppendLine("- 추가: " + DescribeSprite(sprite));
            foreach (string key in plan.removedSpriteKeys) builder.AppendLine("- 제거: " + UIAtlasScanner.DescribeSpriteKey(key));
            builder.AppendLine();
        }

        builder.AppendLine("스프라이트별 상세 목록은 같은 이름의 CSV 파일을 참고");
        return builder.ToString();
    }

    private static string BuildCsv(UIAtlasScanResult result)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Atlas,Sprite,TexturePath,Width,Height,UsageKinds,Sources");
        foreach (UIAtlasPlan plan in result.atlasPlans)
        {
            foreach (Sprite sprite in plan.sprites)
            {
                List<SpriteUsage> usages = result.usagesBySprite[sprite];
                string kinds = string.Join(" ", usages.Select(usage => usage.kind.ToString()).Distinct());
                string sources = string.Join(" | ", usages.Select(usage => usage.sourcePath).Distinct());
                builder.AppendLine(string.Join(",",
                    EscapeCsv(plan.atlas.name),
                    EscapeCsv(sprite.name),
                    EscapeCsv(AssetDatabase.GetAssetPath(sprite)),
                    ((int)sprite.rect.width).ToString(),
                    ((int)sprite.rect.height).ToString(),
                    EscapeCsv(kinds),
                    EscapeCsv(sources)));
            }
        }
        return builder.ToString();
    }

    private static string DescribeSprite(Sprite sprite)
    {
        return AssetDatabase.GetAssetPath(sprite) + " (" + sprite.name + ", " + (int)sprite.rect.width + "x" + (int)sprite.rect.height + ")";
    }

    // 스프라이트를 참조하는 에셋 경로를 최대 개수만큼 이어 붙이는 함수
    private static string DescribeSources(UIAtlasScanResult result, Sprite sprite, int maxCount)
    {
        if (result.usagesBySprite.TryGetValue(sprite, out List<SpriteUsage> usages) == false) return "-";
        List<string> sources = usages.Select(usage => Path.GetFileName(usage.sourcePath)).Distinct().ToList();
        string text = string.Join(", ", sources.Take(maxCount));
        if (sources.Count > maxCount) text += " 외 " + (sources.Count - maxCount) + "곳";
        return text;
    }

    private static string EscapeCsv(string value)
    {
        if (value == null) return string.Empty;
        if (value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) < 0) return value;
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}