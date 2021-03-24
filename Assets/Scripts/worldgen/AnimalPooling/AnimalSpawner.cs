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
        int i = 1;
        while(true)
        {
            pooler.SpawnFromPool("Rabbit", Random.insideUnitCircle * 10, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
