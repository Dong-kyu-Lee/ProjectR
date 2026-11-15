using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PlaytestCheatsWindow : EditorWindow
{
    private static readonly string[] StatNames =
        { "force", "indurance", "critical", "dexterity", "mystery", "curse" };
    private static readonly string[] StatLabels =
        { "무력", "인내", "치명", "재주", "신비", "저주" };

    private Vector2 scroll;
    private int experience = 100;
    private int targetLevel = 5;
    private int statPoints = 1;
    private int selectedStat;
    private int gold = 100;
    private int enchantCharges = 1;

    [MenuItem("Tools/ProjectR/Playtest Cheats")]
    private static void Open()
    {
        GetWindow<PlaytestCheatsWindow>("테스트 치트");
    }

    private void OnInspectorUpdate() => Repaint();

    private void OnGUI()
    {
        if (!EditorApplication.isPlaying || !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorGUILayout.HelpBox("플레이 모드를 시작하면 치트를 사용할 수 있습니다.", MessageType.Info);
            return;
        }

        // 조회만 할 때 플레이어/던전 싱글톤을 새로 만들지 않는다.
        var game = FindFirstObjectByType<GameManager>();
        var playerManager = FindFirstObjectByType<PlayerManager>();
        var player = playerManager != null ? playerManager.CurrentPlayer : null;
        var status = player != null ? player.GetComponent<PlayerStatus>() : null;
        var flow = FindFirstObjectByType<DungeonFlowManager>();
        var stage = flow != null && flow.CurrentStageNumber > 0 ? flow.GetCurrentStage() : null;
        var node = stage != null ? stage.CurrentNode : null;
        var scene = SceneManager.GetActiveScene();

        EditorGUILayout.LabelField("씬", scene.name);
        EditorGUILayout.LabelField("캐릭터", player != null ? playerManager.CurrentCharacterType.ToString() : "없음");
        EditorGUILayout.LabelField("스테이지", node != null
            ? $"{flow.CurrentStageNumber} / {flow.StageCount} · {node.id}"
            : "진행 중인 스테이지 없음");

        string unavailable = GetUnavailableReason(game, player, status);
        if (unavailable != null)
        {
            EditorGUILayout.HelpBox(unavailable, MessageType.Info);
            return;
        }

        using var view = new EditorGUILayout.ScrollViewScope(scroll);
        scroll = view.scrollPosition;
        if (DrawStageControls(flow, stage, scene))
            return;

        DrawGrowthControls(status);
        DrawResourceControls(player, status);
    }

    private static string GetUnavailableReason(GameManager game, GameObject player, PlayerStatus status)
    {
        if (game != null && game.IsSceneTransitionPendingForEditor)
            return "씬 전환 중입니다.";

        if (game == null || player == null || status == null)
            return "플레이어 생성 후 치트를 사용할 수 있습니다.";

        if (!player.activeInHierarchy || status.IsDead || status.Hp <= 0f)
            return "플레이어가 활성화되면 치트를 사용할 수 있습니다.";

        return null;
    }

    private static bool DrawStageControls(DungeonFlowManager flow, Stage stage, Scene scene)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("스테이지 이동", EditorStyles.boldLabel);
        var node = stage != null ? stage.CurrentNode : null;
        bool ready = node != null && stage.isActiveAndEnabled
            && (scene.name == node.SceneName || scene.path == node.SceneName)
            && !EditorApplication.isPaused && Time.timeScale > 0f;
        if (!ready)
            EditorGUILayout.HelpBox("진행 중인 던전에서 일시 정지를 해제하면 이동할 수 있습니다.", MessageType.Info);

        using var disabled = new EditorGUI.DisabledScope(!ready);
        bool lastStage = stage != null && flow.IsLastStage;
        string nextAreaLabel = node != null && node.IsTerminal
            ? (lastStage ? "마지막 구역 완료 · 런 종료" : "마지막 구역 완료 · 다음 스테이지")
            : "다음 구역으로 이동";
        if (GUILayout.Button(nextAreaLabel))
        {
            InGameUIManager.Instance?.HideWarpUI();
            stage.AdvanceAreaForEditor();
            return true;
        }
        if (GUILayout.Button(lastStage ? "마지막 스테이지 종료" : "다음 스테이지로 이동"))
        {
            InGameUIManager.Instance?.HideWarpUI();
            flow.ChangeStage();
            return true;
        }
        return false;
    }

    private void DrawGrowthControls(PlayerStatus player)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("캐릭터 성장", EditorStyles.boldLabel);
        var levels = LevelUp.Instance;
        var upgrades = levels != null ? levels.UpgradeSystemForEditor : null;
        var stats = player.GetComponent<UpgradeStatus>();
        bool levelsReady = levels != null && levels.IsReadyForEditor(player);
        bool upgradesReady = levelsReady && upgrades != null && stats != null;

        EditorGUILayout.LabelField("레벨 / 경험치", $"Lv. {player.Level:0} / {player.Exp:0.##} XP");
        if (!levelsReady || !upgradesReady)
            EditorGUILayout.HelpBox("성장 도구는 던전의 성장 UI가 초기화된 후 사용할 수 있습니다.", MessageType.Info);

        experience = EditorGUILayout.IntField("지급 경험치", experience);
        using (new EditorGUI.DisabledScope(!levelsReady || experience <= 0 || player.Level >= levels.MaxLevelForEditor))
        {
            if (GUILayout.Button("경험치 지급"))
            {
                levels.GrantExpForEditor(experience);
                RefreshCharacterInfo();
            }
        }

        targetLevel = EditorGUILayout.IntField(
            levelsReady ? $"목표 레벨 (최대 {levels.MaxLevelForEditor})" : "목표 레벨", targetLevel);
        using (new EditorGUI.DisabledScope(!levelsReady || targetLevel <= player.Level || targetLevel > levels.MaxLevelForEditor))
        {
            if (GUILayout.Button("목표 레벨까지 성장"))
            {
                levels.GrowToLevelForEditor(targetLevel);
                RefreshCharacterInfo();
            }
        }

        if (stats != null)
            EditorGUILayout.LabelField("남은 스탯 포인트", stats.StatPoint.ToString());
        statPoints = EditorGUILayout.IntField("지급 스탯 포인트", statPoints);
        // 이후 정상 레벨업으로 획득할 포인트의 범위를 남겨 둔다.
        using (new EditorGUI.DisabledScope(!levelsReady || stats == null || statPoints <= 0
            || statPoints > int.MaxValue - LevelUp.requiredExp.Length - stats.StatPoint))
        {
            if (GUILayout.Button("스탯 포인트 지급"))
                levels.GrantStatPointsForEditor(statPoints);
        }
        selectedStat = EditorGUILayout.Popup("투자할 스탯", selectedStat, StatLabels);
        using (new EditorGUI.DisabledScope(!upgradesReady || stats.StatPoint <= 0))
        {
            if (GUILayout.Button("스탯에 포인트 1 투자"))
                upgrades.InvestStatPointForEditor(StatNames[selectedStat]);
        }
    }

    private void DrawResourceControls(GameObject player, PlayerStatus status)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("회복 및 재화", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("체력", $"{status.Hp:0.##} / {status.MaxHp:0.##}");
        if (GUILayout.Button("체력 전체 회복"))
        {
            status.Hp = status.MaxHp;
            RefreshCharacterInfo();
        }

        EditorGUILayout.LabelField("보유 골드", status.Gold.ToString("0.##"));
        gold = EditorGUILayout.IntField("지급 골드", gold);
        using (new EditorGUI.DisabledScope(gold <= 0))
        {
            if (GUILayout.Button("골드 지급"))
                status.Gold += gold;
        }

        if (!player.TryGetComponent(out BlacksmithAbilityV2 blacksmith))
            return;

        EditorGUILayout.LabelField("남은 강화 횟수", blacksmith.UpgradeChance.ToString());
        enchantCharges = EditorGUILayout.IntField("지급 강화 횟수", enchantCharges);
        // 이후 일반 플레이 중 횟수가 추가되어도 정수 범위를 넘지 않도록 한다.
        using (new EditorGUI.DisabledScope(enchantCharges <= 0 || enchantCharges > 999_999 - blacksmith.UpgradeChance))
        {
            if (GUILayout.Button("대장장이 강화 횟수 지급"))
            {
                blacksmith.UpgradeChance += enchantCharges;
                blacksmith.onAbilityUpdated?.Invoke();
            }
        }
    }

    private static void RefreshCharacterInfo()
    {
        var info = InGameUIManager.Instance?.characterInfoUI;
        if (info != null && info.isActiveAndEnabled)
            info.RefreshStatusUI();
    }
}
