using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PresetBuilder))]
public class PresetBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawDefaultInspector();

        PresetBuilder script = (PresetBuilder)target;
        if (GUILayout.Button("Build Preset"))
        {
            script.BuildPreset(CreatePrefab);
        }
        if (GUILayout.Button("Clear Scene"))
        {
            script.ClearWorlds();
        }
    }

    private void CreatePrefab(GameObject root, string name)
    {
        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Worlds/" + name + ".prefab");
        Debug.Log("Complete! Prefab saved in: " + "Assets/World/" + name + ".prefab");
    }
}
