using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

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

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    private List<string> pooledObjects = new List<string> { "Rabbit", "Wolf", "Deer", "Bear" };
    private bool allSpawnedAtStart = false;

    private void Awake()
    {
        instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }
    void Start()
    {
        ObjectPlacement objPl = FindObjectOfType<ObjectPlacement>();
        //objPl.poolEvent.AddListener(HandleAnimalInstantiated);
        objPl.onObjectPlaced += HandleAnimalInstantiated;
        objPl.isDone += HandleFinishedSpawning;
        
        //var rabbits = FindObjectsOfType<RabbitController>();
        //var wolves = FindObjectsOfType<WolfController>();
        //var bears = FindObjectsOfType<BearController>();
        //var deer = FindObjectsOfType<DeerController>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            poolDictionary.Add(pool.tag, objectPool);
        }

        //foreach (var r in rabbits) HandleAnimalInstantiated(r.gameObject, "Rabbits");
        //foreach (var w in wolves) HandleAnimalInstantiated(w.gameObject, "Wolves");
        //foreach (var b in bears) HandleAnimalInstantiated(b.gameObject, "Bears");
        //foreach (var d in deer) HandleAnimalInstantiated(d.gameObject, "Deer");

        Debug.Log("Finishes Start");
    }

    private void HandleFinishedSpawning()
    {
        if (!allSpawnedAtStart)
        {
            allSpawnedAtStart = true;
            
            foreach (Pool pool in pools)
            {
                string objTag = pool.tag;
                for (int i = poolDictionary[objTag].Count; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    poolDictionary[objTag].Enqueue(obj);
                }
            }
            Debug.Log("Finished HandleFinishedSpawning");
        }
    }

    private void HandleAnimalInstantiated(GameObject objectToSpawn, string tag)
    {
        Debug.Log("Handling");
        if (poolDictionary != null && poolDictionary.ContainsKey(tag))
        {
            Debug.Log("Handling");
            objectToSpawn.SetActive(true);
            objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();

            if (objectToSpawn.CompareTag("Animal"))
            {
                objectToSpawn.GetComponent<AnimalController>().Dead += HandleDeadAnimal;
                objectToSpawn.GetComponent<AnimalController>().SpawnNew += HandleBirthAnimal;
            }
            poolDictionary[tag].Enqueue(objectToSpawn);
        }
    }

    private void HandleDeadAnimal(AnimalController animalController)
    {
        GameObject animalObj;
        (animalObj = animalController.gameObject).SetActive(false);
        
        switch (animalController.animalModel)
        {
            case RabbitModel _:
                poolDictionary["Rabbit"].Enqueue(animalObj);
                break;
            case WolfModel _:
                poolDictionary["Wolf"].Enqueue(animalObj);
                break;
            case DeerModel _:
                poolDictionary["Deer"].Enqueue(animalObj);
                break;
            case BearModel _:
                poolDictionary["Bear"].Enqueue(animalObj);
                break;
        }
    }
    
    private void HandleBirthAnimal(AnimalModel childModel, Vector3 pos, float energy, float hydration)
    {
        GameObject child = SpawnFromPool("Rabbit", pos, Quaternion.identity);
        
        AnimalController childController = child.GetComponent<AnimalController>();
        childController.animalModel = childModel;
        childController.animalModel.currentEnergy = energy;
        childController.animalModel.currentHydration = hydration;

        // update the childs speed (in case of mutation).
        childController.animalModel.traits.maxSpeed = 1;
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
                    pool.size += 20;
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
                    objectToSpawn.GetComponent<AnimalController>().SpawnNew += HandleBirthAnimal;
                }

                return objectToSpawn;
        }

        return null;
    }
}