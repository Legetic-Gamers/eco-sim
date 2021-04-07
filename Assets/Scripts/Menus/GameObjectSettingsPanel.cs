using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectSettingsPanel : MonoBehaviour
{
    public TMP_Dropdown objectChooser;
    public Slider prob;
    private GameObjectSelectorFitter gameObjectSelectorFitter;
    [SerializeField]
    private int index;

    private Action OnSave;

    public void Create(int index, GameObjectSettings gameObjectSettings, GameObjectSelectorFitter gameObjectSelectorFitter, Action OnSave)
    {
        PopulateDropdown();
        this.index = index;
        objectChooser.value = gameObjectSettings.GameObjectIndex;
        prob.value = gameObjectSettings.Probability;
        this.gameObjectSelectorFitter = gameObjectSelectorFitter;
        this.OnSave += OnSave;
        OnSave?.Invoke();
    }

    public void SaveSettings()
    {
        OnSave?.Invoke();
    }

    public GameObjectSettings GetCurrentSettings()
    {
        GameObjectSettings gameObjectSettings = new GameObjectSettings(objectChooser.value, prob.value);
        return gameObjectSettings;
    }

    public void Delete()
    {
        gameObjectSelectorFitter.OnGameObjectRemove(index, gameObject);
    }

    private void PopulateDropdown()
    {
        List<string> options = new List<string>();
        for (int i = 0; i < SimulationSettings.instance.availableGameObjects.Length; i++)
        {
            options.Add(SimulationSettings.instance.availableGameObjects[i].SimulationObject.name);
        }
        objectChooser.AddOptions(options);
    }
}
