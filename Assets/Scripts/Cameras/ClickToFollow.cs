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
            animalController.parameterUI.enabled = true;
        }
    }
}
