using System;
using Model;
using UnityEngine;

namespace ViewController
{
    public class PlantController : MonoBehaviour
    {
        public PlantModel plantModel;
        public void Start()
        {
            plantModel = new PlantModel();
        }

        public void Update()
        {
            if (plantModel.isEaten)
            {
                Destroy(gameObject);
            }
        }
    }
}