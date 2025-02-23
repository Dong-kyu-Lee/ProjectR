using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TempMonsterScript))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TempMonsterScript monster = (TempMonsterScript)target;
        if(GUILayout.Button("KillEnemy"))
        {
            monster.Die();
        }
    }
}
