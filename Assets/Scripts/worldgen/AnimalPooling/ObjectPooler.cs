using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataCollection;
using DefaultNamespace;
using Menus;
using Model;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using ViewController;
using Object = UnityEngine.Object;

public class ObjectPooler : MonoBehaviour
{
    /// <summary>
    /// A Pool has a tag for the contained element, rabbit. A prefab and an amount of that object to start with (size)
    /// </summary>
    [Serializable]
    public class Pool : ISerializationCallbackReceiver
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public void OnBeforeSerialize()
        {
            tag = prefab.name.Replace("(Clone)", "").Trim();
        }

        public void OnAfterDeserialize()
        {
            //do nothing
        }
    }

    public static ObjectPooler Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(ObjectPooler)) as ObjectPooler;

            return instance;
        }
        set { instance = value; }
    }

    public static ObjectPooler instance;
    
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Dictionary<string, Stack<GameObject>> stackDictionary;
    private bool showCanvasForAll;
    private bool isFinishedPlacing;
    private DataHandler dh;

    private void Awake()
    {
        instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        stackDictionary = new Dictionary<string, Stack<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            Stack<GameObject> objectStack = new Stack<GameObject>();
            poolDictionary.Add(pool.tag, objectPool);
            stackDictionary.Add(pool.tag, objectStack);
        }
        showCanvasForAll = OptionsMenu.alwaysShowParameterUI;
        isFinishedPlacing = false;
    }

    private void Start()
    {
        dh = FindObjectOfType<DataHandler>();
    }

    /// <summary>
    /// Called when the terrain generator is finished placing all animals and object. 
    /// </summary>
    public void HandleFinishedSpawning()
    {
        if (!isFinishedPlacing)
        {
            GameObject groupObject = new GameObject("Pool");
            foreach (Pool pool in pools)
            {
                string objTag = pool.tag;
                if (stackDictionary[objTag].Count > 0)
                {
                    for (int i = stackDictionary[objTag].Count; i < pool.size; i++)
                    {
                        GameObject obj = Instantiate(pool.prefab, groupObject.transform, true);
                        obj.SetActive(false);
                        poolDictionary[objTag].Enqueue(obj);
                    }

                    for (int i = 0; i < stackDictionary[objTag].Count - 1; i++) poolDictionary[objTag].Enqueue(stackDictionary[objTag].Pop());
                }
            }

            isFinishedPlacing = true;
        }
    }

    /// <summary>
    /// When the terrain generator makes a new animal, the pool handles the animals start method (onObjectSpawn)
    /// and subscribes to the animal mating and death. 
    /// </summary>
    /// <param name="objectToSpawn"> The pooled object the terrain generator will spawn. </param>
    /// <param name="tag"> Tag of the animal, must match names in terrain generator. </param>
    public void HandleAnimalInstantiated(GameObject objectToSpawn, string tag)
    {
        if (poolDictionary != null && poolDictionary.ContainsKey(tag))
        {
            objectToSpawn.SetActive(true);
            objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();
            if (objectToSpawn.TryGetComponent(out AnimalController animalController))
            {
                dh.LogNewAnimal(animalController.animalModel);
                animalController.deadState.onDeath += HandleDeadAnimal;
                animalController.SpawnNew += HandleBirthAnimal;
                animalController.parameterUI.gameObject.SetActive(showCanvasForAll);
            }

            if (stackDictionary != null && stackDictionary.ContainsKey(tag))
            {
                stackDictionary[tag].Push(objectToSpawn);
            }
        }
        
    }

    /// <summary>
    /// Make more space in the queue and disable the animal.
    /// </summary>
    /// <param name="animalController"> Controller of the deceased animal. </param>
    /// <param name="gotEaten"></param>
    public void HandleDeadAnimal(AnimalController animalController, bool gotEaten)
    {
        animalController.deadState.onDeath -= HandleDeadAnimal;
        animalController.SpawnNew -= HandleBirthAnimal;
        if(gotEaten) StartCoroutine(HandleDeadAnimalDelay(animalController, 0f));
        else StartCoroutine(HandleDeadAnimalDelay(animalController, 5f));
    }

    private IEnumerator HandleDeadAnimalDelay(AnimalController animalController, float delay)
    {
        AnimalModel.CauseOfDeath cause;
        AnimalModel am = animalController.animalModel;
        if (am.currentEnergy <= 0) cause = AnimalModel.CauseOfDeath.Energy;
        else if (am.currentHydration <= 0) cause = AnimalModel.CauseOfDeath.Hydration;
        else if (am.age >= am.traits.ageLimit) cause = AnimalModel.CauseOfDeath.Age;
        else if (am.currentHealth <= 0) cause = AnimalModel.CauseOfDeath.Health;
        else cause = AnimalModel.CauseOfDeath.Eaten;
        
        yield return new WaitForSeconds(delay / Time.timeScale);
        if (animalController != null)
        {
            GameObject animalObj;
            (animalObj = animalController.gameObject).SetActive(false);
            
            dh.LogDeadAnimal(am, cause, (transform.position - animalController.startVector).magnitude);
            
            //Debug.Log(animalObj.name.Replace("(Clone)", "").Trim());
            
            poolDictionary[animalObj.name.Replace("(Clone)", "").Trim()].Enqueue(animalObj);
        } 
        
    }

    /// <summary>
    /// Handles animal birth by instantiating the animal as previously using the pool
    /// </summary>
    /// <param name="childModel"> Model to spawn animal with </param>
    /// <param name="pos"> Where the animal is to be spawned </param>
    /// <param name="energy"> The energy of the child </param>
    /// <param name="hydration"> The hydration of the child </param>
    /// <param name="isSmart"> If the animals has ML brain </param>
    private void HandleBirthAnimal(AnimalModel childModel, Vector3 pos, float energy, float hydration, string tag)
    {
        GameObject child;

        child = SpawnFromPool(tag.Replace("(Clone)", "").Trim(), pos, Quaternion.identity);
        
        if (child != null)
        {
            AnimalController childController = child.GetComponent<AnimalController>();
            childController.animalModel = childModel;
            //Debug.Log(childController.animalModel.generation);
            childController.animalModel.currentEnergy = energy;
            childController.animalModel.currentHydration = hydration;
            
            childController.parameterUI.gameObject.SetActive(showCanvasForAll);
            
            // update the childs speed (in case of mutation).
            childController.animalModel.traits.maxSpeed = 1;
            dh.LogNewAnimal(childModel);
        }
    }

    /// <summary>
    /// Activates an object from the given pool named "tag". 
    /// </summary>
    /// <param name="tag"> Name of the pool to spawn from, ie Rabbits</param>
    /// <param name="position"> Where the object is to be spawned </param>
    /// <param name="rotation"> Rotation of the object</param>
    /// <returns></returns>
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
                    for (int i = 0; i < 20; i++)
                    {
                        GameObject obj = Instantiate(pool.prefab);
                        obj.SetActive(false);
                        poolDictionary[tag].Enqueue(obj);
                    }
                }
            }
        }
        
        if (poolDictionary != null && poolDictionary.ContainsKey(tag))
        {
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            if (objectToSpawn != null)
            {
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                objectToSpawn.SetActive(true);
                //TODO Maintain list of all components for more performance
                objectToSpawn.GetComponent<IPooledObject>()?.onObjectSpawn();
                
                if(objectToSpawn.TryGetComponent(out AnimalController animalController))
                {
                    animalController.deadState.onDeath += HandleDeadAnimal;
                    animalController.SpawnNew += HandleBirthAnimal;
                }
                else if (objectToSpawn.TryGetComponent(out PlantController plantController))
                {
                    plantController.SpawnNewPlant += HandleGrowPlant;
                    plantController.onDeadPlant += HandleDeadPlant;
                }
                
                return objectToSpawn;
            }
        }

        return null;
    }

    private void HandleDeadPlant(PlantController plantController)
    {
        GameObject plantObj;
        (plantObj = plantController.gameObject).SetActive(false);
        poolDictionary[plantObj.name.Replace("(Clone)", "").Trim()].Enqueue(plantObj);
        //oolDictionary["Food"].Enqueue(plantObj);
        dh.LogDeadPlant();
        plantController.SpawnNewPlant -= HandleGrowPlant;
        plantController.onDeadPlant -= HandleDeadPlant;
    }

    public void HandleFoodInstantiated(GameObject o, string tag)
    {
        if (poolDictionary != null && poolDictionary.ContainsKey(tag))
        {
            o.SetActive(true);
            o.GetComponent<IPooledObject>()?.onObjectSpawn();
            dh.LogNewPlant();
            if (o.TryGetComponent(out PlantController plantController))
            {
                plantController.onDeadPlant += HandleDeadPlant;
                plantController.SpawnNewPlant += HandleGrowPlant;
            }
            if (stackDictionary != null && stackDictionary.ContainsKey(tag))
            {
                stackDictionary[tag].Push(o);
            }
        }
    }
    
    private void HandleGrowPlant(Vector3 pos)
    {
        GameObject newPlant = SpawnFromPool("Food", pos, Quaternion.identity);
        if (newPlant != null)
        {
            PlantController plantModel = newPlant.GetComponent<PlantController>();
            plantModel.plantModel.isRegrowing = false;
            plantModel.plantModel.plantAge = 0;
            plantModel.plantModel.nutritionValue = 0;
            dh.LogNewPlant();
        } else Debug.Log("Failed to spawn");
    }
}