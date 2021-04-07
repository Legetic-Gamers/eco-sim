using System;
using ICSharpCode.NRefactory.Ast;
using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {

        public bool isEaten = false;
        
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
        
        
    }
    

}