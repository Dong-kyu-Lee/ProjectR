using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySpawnManager))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnemySpawnManager enemySpawnManager = (EnemySpawnManager)target;
        if(GUILayout.Button("KillFirstEnemy"))
        {
            enemySpawnManager.KillFirstWaveEnemy();
        }
        if (GUILayout.Button("KillSecondEnemy"))
        {
            enemySpawnManager.KillSecondWaveEnemy();
        }
    }
}
