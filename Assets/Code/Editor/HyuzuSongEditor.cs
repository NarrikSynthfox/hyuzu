using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hyuzu;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Song))]
public class SongEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (Song)target;

        if(GUILayout.Button("Update Transposes", GUILayout.Height(20)))
            script.TransposeKeys();

        if(GUILayout.Button("Convert Song to JSON", GUILayout.Height(20)))
        {
            string json = JsonUtility.ToJson(script, true);
            File.WriteAllText(script.jsonPath + "/" + script.name + ".json", json);
        }
    }
}