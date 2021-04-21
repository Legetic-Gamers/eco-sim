using System;
using System.Collections;
using System.Linq.Expressions;
using DataCollection;
using DefaultNamespace;
using Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ViewController
{
    public abstract class PlantController : MonoBehaviour, IPooledObject
    {
        public PlantModel plantModel;

        protected DataHandler dh;

        public Transform centerTransform;
        
        public Action<Vector3> SpawnNewPlant;
        public Action<PlantController> onDeadPlant;

        public void Awake()
        {
            if (centerTransform == null)
            {
                Debug.LogWarning("Center not assigned, defaulting to transform");
                centerTransform = transform;
            }
        }

        public void Start()
        {
            //If there is no object pooler present, we need to call onObjectSpawn through start
            if (FindObjectOfType<ObjectPooler>() == null)
            {
                onObjectSpawn();
            }
        }
        
        public abstract void onObjectSpawn();
        
        public abstract float GetEaten();

        public abstract string GetObjectLabel();

    }
}