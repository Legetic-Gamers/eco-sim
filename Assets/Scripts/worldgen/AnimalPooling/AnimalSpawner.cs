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
        //StartCoroutine(SpawnRabbit());
        for (int i = 0; i < 25; i++)
        {
            pooler.SpawnFromPool("Rabbit", new Vector3(Random.Range(0f, 10f), 0, Random.Range(0f, 10f)), Quaternion.identity);
        }
    }

    private IEnumerator SpawnRabbit()
    {
        while(true)
        {
            Vector3 position = new Vector3(Random.Range(0f, 10f), 0, Random.Range(0f, 10f));
            pooler.SpawnFromPool("Rabbit", position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
    }
}
