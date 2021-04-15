using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(PresetBuilder))]
public class PresetBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

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

    private void CreatePrefab(GameObject root, string name, NavMeshData navMeshData)
    {
        PresetBuilder script = (PresetBuilder)target;
        var path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Worlds/" + name + ".prefab");
        PrefabUtility.SaveAsPrefabAsset(root, path);
        AssetDatabase.CreateAsset(navMeshData, AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Worlds/NavMesh-" + name + ".asset"));
        Debug.Log("Complete! Prefab saved in: " + path);
        script.ResetTransformSettings();

    }
}
