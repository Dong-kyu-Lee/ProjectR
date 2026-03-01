using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSceneBootstrap : MonoBehaviour
{
    [Header("Spawn & Prefabs")]
    public Transform playerSpawn, enemySpawn;
    public GameObject playerPrefab, enemyPrefab;

    [Header("UI Panels")]
    public StatusPanel playerStatusPanel, enemyStatusPanel;
    public ControllerPanel controllerPanel;
    public UpgradePanelBinder upgradePanel; // 선택

    private GameObject player, enemy;

    void Start()
    {
        player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        enemy = Instantiate(enemyPrefab, enemySpawn.position, Quaternion.identity);

        var pStatus = player.GetComponent<Status>();
        var eStatus = enemy.GetComponent<Status>();

        playerStatusPanel.Bind(pStatus, "PLAYER");
        enemyStatusPanel.Bind(eStatus, "ENEMY");

        controllerPanel.Bind(pStatus);                // ← 스탯만 조작
        if (upgradePanel) upgradePanel.Bind(player);  // ← 선택: 업그레이드 시스템 연결
    }
}
