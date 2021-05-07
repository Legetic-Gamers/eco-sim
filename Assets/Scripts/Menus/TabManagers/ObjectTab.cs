using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectTab : SettingsManager
{
    public int currentlySelectedIndex = 0;
    public TMP_Dropdown objectTypeSelectorDropdown;

    public TMP_InputField nameField;
    public TMP_InputField minimumDistanceField;
    public TMP_InputField newPointCountField;
    public TMP_InputField scaleField;
    public TMP_InputField yOffsetField;
    public Slider minHeightSlider;
    public Slider maxHeightSlider;

    public GameObjectSelectorFitter fitter;

    private bool isStarting = true;
    private bool isBlocked = false;

    protected override void Start()
    {
        base.Start();
        isStarting = true;
        fitter.OnInnerSettingsSaved = SaveValuesToSettings;
        UpdateDropdownOptions();
        if (simulationSettings.ObjectPlacementSettings.ObjectTypes.Count > 0)
        {
            currentlySelectedIndex = 0;
            SetValuesToFields(simulationSettings.ObjectPlacementSettings.GetObjectType(currentlySelectedIndex));
            fitter.Populate(simulationSettings.ObjectPlacementSettings.GetObjectType(currentlySelectedIndex));
        }
        isStarting = false;
    }

    public void AddObjectType()
    {
        simulationSettings.ObjectPlacementSettings.AddType(
            new ObjectType(
                "New object " + simulationSettings.ObjectPlacementSettings.ObjectTypes.Count,
                new GameObjectSettings[] { new GameObjectSettings(0, 1) },
                10,
                10,
                1f,
                0,
                0,
                1
            )
        );
        UpdateDropdownOptions();
        ChangeSelection(simulationSettings.ObjectPlacementSettings.ObjectTypes.Count - 1);
    }

    public void UpdateObjectType(ObjectType objectType)
    {
        simulationSettings.ObjectPlacementSettings.UpdateIndex(
            currentlySelectedIndex, objectType
        );
    }

    public void DeleteObjectType()
    {
        simulationSettings.ObjectPlacementSettings.RemoveTypeIndex(currentlySelectedIndex);
        UpdateDropdownOptions();
        if (currentlySelectedIndex > 0)
        {
            ChangeSelection(currentlySelectedIndex - 1);
        }
        else
        {
            ChangeSelection(0);
        }
    }

    public void ChangeSelection()
    {
        ChangeSelection(objectTypeSelectorDropdown.value);
    }

    public void ChangeSelection(int index)
    {
        isBlocked = true;
        if (index >= 0 && index < simulationSettings.ObjectPlacementSettings.ObjectTypes.Count)
        {
            currentlySelectedIndex = index;
            objectTypeSelectorDropdown.value = currentlySelectedIndex;
            ObjectType objectType = simulationSettings.ObjectPlacementSettings.GetObjectType(currentlySelectedIndex);
            SetValuesToFields(objectType);
            fitter.Populate(objectType);
        }
        isBlocked = false;
    }

    public void SetValuesToFields(ObjectType objectType)
    {
        SetValuesToFields(objectType.Name, objectType.MinimumDistance, objectType.NewPointCount, objectType.Scale, objectType.yOffset, objectType.MinHeight, objectType.MaxHeight);
    }

    public void SetValuesToFields(string name, float minimumDistance, int newPointCount, float scale, float yOffset, float minHeight, float maxHeight)
    {
        nameField.text = name;
        minimumDistanceField.text = minimumDistance.ToString();
        newPointCountField.text = newPointCount.ToString();
        scaleField.text = scale.ToString();
        yOffsetField.text = yOffset.ToString();
        minHeightSlider.value = minHeight;
        maxHeightSlider.value = maxHeight;
    }

    public void SaveValuesToSettings()
    {
        if (isStarting || isBlocked)
        {
            print("No update made! blocked: " + isBlocked + " is starting: " + isStarting);
            return;
        }

        

        print("Changing the placement!");
        ObjectType newObjectType = new ObjectType(
            nameField.text,
            fitter.GetCurrentSettings(),
            float.Parse(minimumDistanceField.text),
            int.Parse(newPointCountField.text),
            float.Parse(scaleField.text),
            float.Parse(yOffsetField.text),
            minHeightSlider.value,
            maxHeightSlider.value
        );

        simulationSettings.ObjectPlacementSettings.UpdateIndex(currentlySelectedIndex, newObjectType);
    }

    public void UpdateAllObjectPlacement()
    {
        //Update each object placed.
        // for (int index = 0;
        // index < simulationSettings.ObjectPlacementSettings.ObjectTypes.Count;
        // index++)
        // {
        //     
        //     //Get the old object settings
        //     ObjectType oldObjectType = simulationSettings.ObjectPlacementSettings.ObjectTypes[index];
        //     
        //     ObjectType newObjectType = new ObjectType(
        //         oldObjectType.Name,
        //         oldObjectType.GameObjectSettings.ToArray(),
        //         oldObjectType.MinimumDistance,
        //         oldObjectType.NewPointCount,
        //         oldObjectType.Scale,
        //         oldObjectType.yOffset,
        //         oldObjectType.MinHeight,
        //         oldObjectType.MaxHeight
        //     );
        //     
        //     simulationSettings.ObjectPlacementSettings.UpdateIndex(currentlySelectedIndex, newObjectType);
        // }
        
        simulationSettings.ObjectPlacementSettings.InvokeUpdates();

    }

    public void UpdateDropdownOptions()
    {
        List<string> names = new List<string>();
        var objectTypes = simulationSettings.ObjectPlacementSettings.ObjectTypes;

        for (int i = 0; i < objectTypes.Count; i++)
        {
            names.Add(i + ": " + objectTypes[i].Name);
            // Debug.Log("Adding name: " + i + ": " + objectTypes[i].Name);
        }

        objectTypeSelectorDropdown.ClearOptions();
        objectTypeSelectorDropdown.AddOptions(names);
    }

}
