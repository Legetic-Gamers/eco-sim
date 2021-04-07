using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameObjectSelectorFitter : MonoBehaviour
{
    public float sizeOfElements;
    public GameObject gameObjectSettingsPanel;
    //public ObjectType currentObjectType;

    public List<GameObject> panels = new List<GameObject>();

    public Action OnInnerSettingsSaved;

    public void Populate(ObjectType objectType)
    {
        Clear();
        Debug.Log("Populate the gameobjectlist");
        var gameObjectSettings = objectType.GameObjectSettings;
        Debug.Log("Name was " + objectType.Name + " and length was " + gameObjectSettings.Count);
        for (int i = 0; i < gameObjectSettings.Count; i++)
        {
            InstantiateGameObjectManager(gameObjectSettings[i]);
        }
    }

    public void OnGameObjectAdd()
    {
        InstantiateGameObjectManager(new GameObjectSettings(0, 1f));
    }

    public void OnGameObjectRemove(int index, GameObject panel)
    {
        panels.Remove(panel);
        SimulationSettings.instance.ObjectPlacementSettings.RemoveTypeIndex(index);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        if (rect.rect.height >= sizeOfElements)
        {
            rect.sizeDelta -= new Vector2(0, sizeOfElements);
        }
    }

    public GameObjectSettings[] GetCurrentSettings()
    {
        List<GameObjectSettings> gameObjectSettingsList = new List<GameObjectSettings>();
        for (int i = 0; i < panels.Count; i++)
        {
            var panelScript = panels[i].GetComponent<GameObjectSettingsPanel>();
            if (panelScript != null)
                gameObjectSettingsList.Add(panelScript.GetCurrentSettings());
            else
                Debug.LogError("Shit be crazy");
        }
        return gameObjectSettingsList.ToArray();
    }

    private void InstantiateGameObjectManager(GameObjectSettings gameObjectSettings)
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta += new Vector2(0, sizeOfElements);

        GameObject obj = Instantiate(gameObjectSettingsPanel, transform);
        panels.Add(obj);
        GameObjectSettingsPanel panel = obj.GetComponent<GameObjectSettingsPanel>();

        panel.Create(panels.IndexOf(obj), gameObjectSettings, this, OnInnerSettingsSaved);
    }

    private void Clear()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.rect.width, 0);
        panels.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
