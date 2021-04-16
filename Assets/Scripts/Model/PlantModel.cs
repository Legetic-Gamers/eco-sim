using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {
        public bool isEaten = false;

        public float nutritionValue { get; set; }
        public float plantAge;

        public const float plantMaxAge = 60;
        public const float plantMaxsize = 30;
        
        public PlantModel(float nutritionValue)
        {
            this.nutritionValue = nutritionValue;
        }

        public PlantModel()
        {
            this.nutritionValue = 0;
            this.plantAge = 0;
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