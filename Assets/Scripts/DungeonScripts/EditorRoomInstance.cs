using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomInstance))]
public class EditorRoomInstance : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RoomInstance roomInstance = (RoomInstance)target;
        if (GUILayout.Button("End Rooms"))
        {
            roomInstance.Editor_EndRoom();
        }
    }
}
