using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FoVEdit : Editor
{
    private void OnSceneGUI() 
    {
        var animal = (FieldOfView)target;
        Handles.color = Color.white;

        var animalPos = animal.transform.position;
        
        // Draw perpendicular circles around the animal
        Handles.DrawWireArc(animalPos, Vector3.up, Vector3.forward, 360, animal.radius);
        Handles.DrawWireArc(animalPos, Vector3.forward, Vector3.up, 360, animal.radius);
        
        Vector3 viewAngleA = animal.DirectionOfAngle(-animal.angle / 2);
        Vector3 viewAngleB = animal.DirectionOfAngle(animal.angle / 2);
        // Draw view cone lines
        Handles.DrawLine(animalPos, animalPos + viewAngleA * animal.radius);
        Handles.DrawLine(animalPos, animalPos + viewAngleB * animal.radius);

        // Draw line from animal to target
        Handles.color = Color.red;
        foreach (Transform target in animal.targets)
        {
            Handles.DrawLine(animal.transform.position, target.position);
        }
    }
}
