using System;
using ICSharpCode.NRefactory.Ast;
using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {

        public bool isEaten = false;
<<<<<<< HEAD
        
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
=======
        public float plantSize;
        public float plantAge;

        public const float plantMaxAge = 60;
        public const float plantMaxsize = 30;
        
        public float GetEaten()
        {
            isEaten = true;
            float tmp = plantSize;
            plantSize = 0;
            return tmp;
>>>>>>> dynamic-food
        }
        
        public PlantModel()
        {
            this.plantAge = 0;
            this.plantSize = 0;
        }
        
        
    }
    

}