using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimalSpawner : MonoBehaviour
{
    private ObjectPooler pooler;
    public void Start()
    {
        pooler = ObjectPooler.instance;
        StartCoroutine(SpawnRabbit());
    }

    private IEnumerator SpawnRabbit()
    {
        while(true)
        {
            Vector3 position = Random.insideUnitCircle * 10;
            pooler.SpawnFromPool("Rabbit", position, Quaternion.identity);
            Debug.Log(position);
            yield return new WaitForSeconds(5f);
        }
    }
}
