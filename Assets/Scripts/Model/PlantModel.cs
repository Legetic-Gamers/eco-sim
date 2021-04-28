using System;
using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {
        public Action onGrowOld;

        public float _nutritionValue;
        public float nutritionValue
        {
            get
            {
                return _nutritionValue;
            }
            set
            {
                if (value > plantMaxNutrition)
                {
                    _nutritionValue = plantMaxNutrition;
                }
                else
                {
                    _nutritionValue = value;
                }
            }
        }

        public float _plantAge;
        public float plantAge
        {
            get
            {
                return _plantAge;
            }
            set
            {
                if (value > plantMaxAge)
                {
                    _plantAge = plantMaxAge;
                }
                else
                {
                    _plantAge = value;
                }
            }
        }

        public bool isMature => !isRegrowing && nutritionValue > plantMaxNutrition/2;
        
        public bool isRegrowing;

        public const float plantMaxAge = 60;
        public const float plantMaxNutrition = 60;
        
        public PlantModel(float nutritionValue, float plantAge)
        {
            this.plantAge = plantAge;
            isRegrowing = false;
            this.nutritionValue = nutritionValue;
        }

        public PlantModel()
        {
            nutritionValue = 0;
            plantAge = 0;
            isRegrowing = false;
        }

        public void Grow()
        {
            plantAge += 1f;
            nutritionValue += 1;
            if (plantAge >= plantMaxAge)
            {
                onGrowOld?.Invoke();
            }
            
        }
       
        public float GetEaten()
        {
            float tmp = 0;
            if (!isRegrowing)
            {
                tmp = nutritionValue;
                nutritionValue = 0;
                isRegrowing = true;
            }
            return tmp;
        }
        
    }
}