using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HearingAbility))]
public class FoHEdit : Editor
{
    private void OnSceneGUI()
    {
        var animal = (HearingAbility)target;
        Handles.color = Color.yellow;

        var animalPos = animal.transform.position;

        // Draw perpendicular circles around the animal
        Handles.DrawWireArc(animalPos, Vector3.up, Vector3.forward, 360, animal.radius);
        Handles.DrawWireArc(animalPos, Vector3.forward, Vector3.up, 360, animal.radius);

        // Draw line from animal to target
        Handles.color = Color.blue;
        foreach (var target in animal.targets)
        {
            Handles.DrawLine(animal.transform.position, target.position);
        }
    }
}
