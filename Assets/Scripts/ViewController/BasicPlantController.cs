﻿using DataCollection;
using Model;

namespace ViewController
{
    public class BasicPlantController : PlantController
    {
        public override void onObjectSpawn()
        {
            plantModel = new PlantModel(35f, 40, 80, 60);
        }

        public override float GetEaten()
        {
            float nutrition = plantModel.GetEaten();
            onDeadPlant?.Invoke(this);
            if (!FindObjectOfType<ObjectPooler>())
            {
                //Destroy(gameObject);
            }
            return nutrition;
        }
        
        
        public override string GetObjectLabel()
        {
            return "BasicPlant";
        }
    }
}