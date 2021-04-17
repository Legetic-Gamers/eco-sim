﻿using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {
        public bool isEaten;

        public float nutritionValue { get; set; }
        public float plantAge;

        public const float plantMaxAge = 60;
        public const float plantMaxsize = 60;
        
        public PlantModel(float nutritionValue)
        {
            nutritionValue = nutritionValue;
        }

        public PlantModel()
        {
            nutritionValue = 0;
            plantAge = 0;
            isEaten = false;
        }
       
        public float GetEaten()
        {
            isEaten = true;
            float tmp = nutritionValue;
            nutritionValue = 0;
            return tmp;
        }

    }
}