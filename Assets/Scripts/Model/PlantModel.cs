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
            nutritionValue = nutritionValue;
        }

        public PlantModel()
        {
            nutritionValue = 30f;
        }
       
        public float GetEaten()
        {
            isEaten = true;
            return nutritionValue;
        }

    }
}