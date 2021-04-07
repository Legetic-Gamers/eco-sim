using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {
        public bool isEaten = false;
        
<<<<<<< HEAD
        public float plantAge;

        public const float plantMaxAge = 60;
        public const float plantMaxsize = 30;


        public float nutritionValue { get; set; }

        public float GetEaten()
        {
            isEaten = true;
            float tmp = nutritionValue;
            nutritionValue = 0;
            return tmp;

        }
        
        public PlantModel()
        {
            this.plantAge = 0;
            this.nutritionValue = 0;
        }
        
        
=======
        public float nutritionValue { get; set; }

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
>>>>>>> parent of e186d122 (Merge branch 'dynamic-food' into develop)
    }
}