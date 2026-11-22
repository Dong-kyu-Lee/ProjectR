using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PlaytestCheatsWindow : EditorWindow
{
    private static readonly string[] StatNames =
        { "force", "indurance", "critical", "dexterity", "mystery", "curse" };
    private static readonly string[] StatLabels =
        { "무력", "인내", "치명", "재주", "신비", "저주" };
    private static readonly string[] WeaponGradeLabels = { "C", "B", "A", "S" };

    private Vector2 scroll;
    private int experience = 100;
    private int targetLevel = 5;
    private int statPoints = 1;
    private int selectedStat;
    private int gold = 100;
    private int enchantCharges = 1;
    private int bottleCount = 10;
    private BuffType selectedBuff = BuffType.AttackDamageIncrease;
    private float buffDuration = 10f;
    private BasicItemData itemToGrant;
    private int itemAmount = 1;
    private int weaponIndex = 1;
    private int weaponRank = 1;
    private int weaponLevel;
    private int targetStageIndex;
    private int targetNodeIndex;
    private string grantMessage;
    private float healthPercent = 50f;

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
        DrawCombatControls(player, status, stage, scene);
        DrawResourceControls(player, status);
        DrawItemControls(player.GetComponentInChildren<Inventory>());
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

    private bool DrawStageControls(DungeonFlowManager flow, Stage stage, Scene scene)
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
        if (flow == null || flow.StageCount == 0) return false;

        var stageLabels = new string[flow.StageCount];
        for (int i = 0; i < stageLabels.Length; i++)
            stageLabels[i] = $"{i + 1} · {flow.SelectedStageDatas[i].name}";
        targetStageIndex = Mathf.Clamp(targetStageIndex, 0, stageLabels.Length - 1);
        int selectedStage = EditorGUILayout.Popup("이동할 스테이지", targetStageIndex, stageLabels);
        if (selectedStage != targetStageIndex)
        {
            targetStageIndex = selectedStage;
            targetNodeIndex = 0;
        }
        var nodes = new List<StageNode>(
            new StageGraph(flow.SelectedStageDatas[targetStageIndex]).NodesForEditor);
        if (nodes.Count == 0) return false;
        var nodeLabels = new string[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
            nodeLabels[i] = $"{nodes[i].id} · {nodes[i].type}";
        targetNodeIndex = Mathf.Clamp(targetNodeIndex, 0, nodes.Count - 1);
        targetNodeIndex = EditorGUILayout.Popup("이동할 구역 / 보스", targetNodeIndex, nodeLabels);
        using (new EditorGUI.DisabledScope(!Application.CanStreamedLevelBeLoaded(nodes[targetNodeIndex].SceneName)))
        {
            if (GUILayout.Button("선택 구역으로 바로 이동"))
            {
                InGameUIManager.Instance?.HideWarpUI();
                flow.JumpToNodeForEditor(targetStageIndex + 1, nodes[targetNodeIndex].id);
                return true;
            }
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
        healthPercent = EditorGUILayout.Slider("지정할 체력 (%)", healthPercent, 0f, 100f);
        if (GUILayout.Button(healthPercent == 0f ? "체력 0% 적용 · 사망" : "지정 비율로 체력 변경"))
        {
            status.SetHealthPercentForEditor(healthPercent);
            RefreshCharacterInfo();
        }

        EditorGUILayout.LabelField("보유 골드", status.Gold.ToString("0.##"));
        gold = EditorGUILayout.IntField("지급 골드", gold);
        using (new EditorGUI.DisabledScope(gold <= 0))
        {
            if (GUILayout.Button("골드 지급"))
                status.Gold += gold;
        }

        if (player.TryGetComponent(out BartenderAbilityV2 bartender))
        {
            EditorGUILayout.LabelField("보유 특수 병", bartender.GetBottleCounts().ToString());
            bottleCount = EditorGUILayout.IntField("지급 특수 병 수", bottleCount);
            using var disabled = new EditorGUI.DisabledScope(bottleCount <= 0
                || bottleCount > 10_000 - bartender.GetBottleCounts());
            if (GUILayout.Button("바텐더 특수 병 지급"))
                bartender.AddBartenderBottle(bottleCount);
        }

        if (!player.TryGetComponent(out BlacksmithAbilityV2 blacksmith))
            return;

        DrawBlacksmithWeaponControls(blacksmith);
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

    private void DrawItemControls(Inventory inventory)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("아이템 및 장비 지급", EditorStyles.boldLabel);
        itemToGrant = (BasicItemData)EditorGUILayout.ObjectField("지급할 아이템", itemToGrant, typeof(BasicItemData), false);
        bool consumable = itemToGrant is ConsumableItemData && itemToGrant.ItemType == ItemType.CONSUMABLE;
        bool equipment = itemToGrant is EquipmentItemData && itemToGrant.ItemType == ItemType.EQUIPMENT;
        if (consumable)
            itemAmount = EditorGUILayout.IntField("지급 수량", itemAmount);
        if (equipment)
            EditorGUILayout.LabelField("지급 수량", "1개 · 장비칸이 비어 있으면 자동 장착");
        using (new EditorGUI.DisabledScope(inventory == null || (!consumable && !equipment)
            || (consumable && itemAmount <= 0)))
        {
            if (GUILayout.Button("선택 아이템 지급"))
            {
                bool success = inventory.GrantItemForEditor(itemToGrant, equipment ? 1 : itemAmount);
                grantMessage = success ? "아이템을 지급했습니다." : "지급할 공간 또는 아이템 설정을 확인하세요.";
                RefreshCharacterInfo();
            }
        }
        if (!string.IsNullOrEmpty(grantMessage))
            EditorGUILayout.HelpBox(grantMessage, MessageType.Info);
    }

    private void DrawBlacksmithWeaponControls(BlacksmithAbilityV2 ability)
    {
        var weapons = ability.WeaponsForEditor;
        if (weapons == null || weapons.Count <= 1) return;
        if (ability.CurWeaponData != null)
            EditorGUILayout.LabelField("현재 무기", ability.IsActivated
                ? $"{ability.CurWeaponData.WeaponName} · {WeaponGradeLabels[Mathf.Clamp(ability.CurWeaponData.Rank, 1, 4) - 1]} · +{ability.EnchantLevel}"
                : ability.CurWeaponData.WeaponName);
        var labels = new string[weapons.Count - 1];
        for (int i = 1; i < weapons.Count; i++)
            labels[i - 1] = weapons[i] != null ? weapons[i].WeaponName : "무기 없음";
        weaponIndex = Mathf.Clamp(weaponIndex, 1, weapons.Count - 1);
        weaponIndex = EditorGUILayout.Popup("지정할 무기", weaponIndex - 1, labels) + 1;
        weaponRank = EditorGUILayout.Popup("무기 등급", weaponRank - 1, WeaponGradeLabels) + 1;
        weaponLevel = EditorGUILayout.IntSlider("강화 단계", weaponLevel, 0, 5);
        using var disabled = new EditorGUI.DisabledScope(ability.IsEnchanting || weapons[weaponIndex] == null);
        if (GUILayout.Button("무기 상태 적용"))
        {
            ability.SetWeaponForEditor(weapons[weaponIndex], weaponRank, weaponLevel);
            RefreshCharacterInfo();
        }
    }

    private void DrawCombatControls(GameObject player, PlayerStatus status, Stage stage, Scene scene)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("전투 테스트", EditorStyles.boldLabel);
        status.InvincibleForEditor = EditorGUILayout.Toggle("치트 무적", status.InvincibleForEditor);

        DrawBuffControls(player.GetComponent<BuffManager>());

        RoomInstance room = FindCurrentRoom(stage, scene, player.transform.position);
        var enemies = room != null ? room.GetComponent<EnemyInRoom>() : null;
        bool clearing = enemies != null && enemies.IsClearingForEditor;
        EditorGUILayout.LabelField("현재 방", room != null ? room.name : "일반 던전 방 안에서 사용 가능");
        using var disabled = new EditorGUI.DisabledScope(room == null || room.GetRoomState != RoomState.Start
            || enemies == null || clearing || EditorApplication.isPaused || Time.timeScale <= 0f);
        if (GUILayout.Button(clearing ? "현재 방의 웨이브 처치 중…" : "현재 방의 적 모두 처치"))
            enemies.ClearRoomForEditor();
    }

    private void DrawBuffControls(BuffManager manager)
    {
        if (manager == null) return;

        selectedBuff = (BuffType)EditorGUILayout.EnumPopup("버프 / 디버프", selectedBuff);
        buffDuration = EditorGUILayout.FloatField("적용 지속시간 (초)", buffDuration);
        using (new EditorGUI.DisabledScope(selectedBuff == BuffType.None
            || !float.IsFinite(buffDuration) || buffDuration <= 0f))
        {
            if (GUILayout.Button("선택 효과 적용 / 중첩"))
            {
                var buff = manager.GenerateBuff(selectedBuff, buffDuration, manager.gameObject);
                float duration = Mathf.Min(buffDuration, buff.MaxDuration);
                if (buff.IsDebuff)
                    manager.ActivateDeBuff(selectedBuff, duration);
                else
                    manager.ActivateBuff(selectedBuff, duration);

                if (manager.ActiveBuffDict.TryGetValue(selectedBuff, out var active))
                    active.CurrentDuration = Mathf.Min(active.CurrentDuration, active.MaxDuration);
                RefreshCharacterInfo();
            }
        }
        using (new EditorGUI.DisabledScope(!manager.ActiveBuffDict.ContainsKey(selectedBuff)))
        {
            if (GUILayout.Button("선택 효과 해제"))
            {
                manager.DeactivateBuff(selectedBuff);
                RefreshCharacterInfo();
            }
        }
        using (new EditorGUI.DisabledScope(manager.ActiveBuffDict.Count == 0))
        {
            if (GUILayout.Button("모든 버프 / 디버프 해제"))
            {
                foreach (var type in new List<BuffType>(manager.ActiveBuffDict.Keys))
                    manager.DeactivateBuff(type);
                RefreshCharacterInfo();
            }
        }

        foreach (var buff in manager.GetActiveBuffs())
            EditorGUILayout.LabelField(buff.BuffType.ToString(),
                $"Lv. {buff.CurrentBuffLevel + 1} · {buff.CurrentDuration:0.0}초");
    }

    private static RoomInstance FindCurrentRoom(Stage stage, Scene scene, Vector3 position)
    {
        if (stage == null || !stage.isActiveAndEnabled || stage.CurrentNode == null
            || stage.CurrentNode.type != StageNodeType.NormalDungeon
            || (scene.name != stage.CurrentNode.SceneName && scene.path != stage.CurrentNode.SceneName))
            return null;

        foreach (var room in stage.roomList)
        {
            if (room != null && room.TryGetComponent(out Collider2D area) && area.OverlapPoint(position))
                return room;
        }
        return null;
    }

    private static void RefreshCharacterInfo()
    {
        var info = InGameUIManager.Instance?.characterInfoUI;
        if (info != null && info.isActiveAndEnabled)
            info.RefreshStatusUI();
    }
}
