using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler instance;

    #endregion

    private void Awake()
    {
        instance = this;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
        Debug.Log(poolDictionary["Rabbit"].Count);
    }

    private void HandleDeadAnimal(AnimalController animalController)
    {
        switch (animalController.animalModel)
        {
            case RabbitModel _:
                GameObject rabbitObj;
                (rabbitObj = animalController.gameObject).SetActive(false);
                poolDictionary["Rabbit"].Enqueue(rabbitObj);
                Debug.Log("Died");
                Debug.Log(poolDictionary["Rabbit"].Count);
                break;
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary[tag].Any())
        {
            // Empty queue, make more
            foreach (var pool in pools)
            {
                if (pool.tag.Equals(tag))
                {
                    pool.size++;
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    poolDictionary[tag].Enqueue(obj);
                }
            }
        }

        if (poolDictionary != null && poolDictionary.ContainsKey(tag))
        {
                GameObject objectToSpawn = poolDictionary[tag].Dequeue();

                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                objectToSpawn.SetActive(true);

                //TODO Maintain list of all components for more performance
                objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();

                if (objectToSpawn.CompareTag("Animal"))
                {
                    objectToSpawn.GetComponent<AnimalController>().Dead += HandleDeadAnimal;
                }

                return objectToSpawn;
        }

        return null;
    }
}