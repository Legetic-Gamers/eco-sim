using System;
using DataCollection;
using Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ViewController
{
    public abstract class PlantController : MonoBehaviour
    {
        public PlantModel plantModel;
        public Transform centerTransform;

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
            plantModel = new PlantModel();
        }
        
        public abstract float GetEaten();
    
        
    }
}