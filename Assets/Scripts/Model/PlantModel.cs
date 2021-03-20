using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {
        public bool isEaten = false;
        
        public float nutritionValue { get; set; }

        public PlantModel(float nutritionValue)
        {
            nutritionValue = nutritionValue;
        }

        public PlantModel()
        {
            nutritionValue = 15f;
        }
       
        public float GetEaten()
        {
            isEaten = true;
            return nutritionValue;
        }
    }
}