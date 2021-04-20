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
                if (value > plantMaxsize)
                {
                    _nutritionValue = plantMaxsize;
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
        
        public bool isRegrowing;

        public const float plantMaxAge = 60;
        public const float plantMaxsize = 50;
        
        public PlantModel(float nutritionValue)
        {
            plantAge = 0;
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
            plantAge += 2;
            nutritionValue += 4;
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