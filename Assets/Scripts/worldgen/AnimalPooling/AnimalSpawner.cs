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
        Vector3 p1 = new Vector3(Random.Range(0f, 10f), 0, Random.Range(0f, 10f));
        pooler.SpawnFromPool("Rabbit", p1, Quaternion.identity);
        Vector3 p2 = new Vector3(Random.Range(0f, 10f), 0, Random.Range(0f, 10f));
        pooler.SpawnFromPool("Rabbit", p2, Quaternion.identity);
        Vector3 p3 = new Vector3(Random.Range(0f, 10f), 0, Random.Range(0f, 10f));
        pooler.SpawnFromPool("Rabbit", p3, Quaternion.identity);
        
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
