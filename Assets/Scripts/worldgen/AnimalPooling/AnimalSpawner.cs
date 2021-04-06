using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimalSpawner : MonoBehaviour
{
    private ObjectPooler pooler;
    public Action<GameObject, string> onAnimalInstantiated;
    public void Start()
    {
        pooler = ObjectPooler.instance;
        
        //StartCoroutine(SpawnRabbit());
        for (int i = 0; i < 2; i++)
        {
            //pooler.SpawnFromPool("Rabbit", new Vector3(Random.Range(0f, 10f), 0, Random.Range(0f, 10f)), Quaternion.identity);
            GameObject obj = Instantiate(pooler.pools[0].prefab, new Vector3(Random.Range(0f, 10f), 0, 0), Quaternion.identity);
            obj.SetActive(false);
            onAnimalInstantiated?.Invoke(obj, "Rabbit");
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
