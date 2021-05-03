using System.Collections;
using System.Collections.Generic;
using Menus;
using UnityEngine;

public class ClickToFollow : MonoBehaviour
{
    private void OnMouseDown()
    {
        OrbitCameraController.instance.followTransform = transform;
        if (TryGetComponent(out AnimalController animalController))
        {
            OrbitCameraController.instance.Transition();
            OrbitCameraController.instance.animalController?.GetComponentInChildren<ParameterUI>(true).SetUIActive(OptionsMenu.alwaysShowParameterUI);
            OrbitCameraController.instance.animalController = animalController;
            animalController.GetComponentInChildren<ParameterUI>(true).SetUIActive(true);   //set new parameterUI active
        }
    }
}
