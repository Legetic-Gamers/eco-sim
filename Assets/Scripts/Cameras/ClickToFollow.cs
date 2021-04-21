using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToFollow : MonoBehaviour
{
    private void OnMouseDown()
    {
        OrbitCameraController.instance.followTransform = transform;
        if (TryGetComponent(out AnimalController animalController))
        {
            OrbitCameraController.instance.Transition();
            OrbitCameraController.instance.animalController?.parameterUI.gameObject.SetActive(false);
            OrbitCameraController.instance.animalController = animalController;
            animalController.parameterUI.gameObject.SetActive(true);
        }
    }
}
