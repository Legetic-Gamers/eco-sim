using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {
        public bool isEaten;

        public float nutritionValue { get; set; }
        public float plantAge;
        public bool isRegrowing;

        public const float plantMaxAge = 60;
        public const float plantMaxsize = 50;
        
        public PlantModel(float nutritionValue)
        {
            this.nutritionValue = nutritionValue;
        }

        public PlantModel()
        {
            nutritionValue = 0;
            plantAge = 0;
            isEaten = false;
            isRegrowing = false;
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