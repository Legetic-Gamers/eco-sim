using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

/// <summary>
/// This class is heavily inspired by (mostly copied) from the ML-Toolkit FoodCollectorArea class.
/// https://github.com/Unity-Technologies/ml-agents/blob/main/Project/Assets/ML-Agents/Examples/FoodCollector/Scripts/FoodCollectorArea.cs
/// Used to randomly spawn things in the ML testing environment and reset.
/// </summary>
public class World : MonoBehaviour
{
    public GameObject food;
    public GameObject water;
    public int numFood;
    public int numWater;

    public float range;

    
    private void CreateObjects(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, new Vector3(Random.Range(-range, range), 0,
                    Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
        }
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (var food in objects)
        {
            Destroy(food);
        }
    }

    public void ResetWorld()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("Plant"));
        ClearObjects(GameObject.FindGameObjectsWithTag("Water"));
        
        CreateObjects(numFood, food);
        CreateObjects(numWater, water);
    }
    
}