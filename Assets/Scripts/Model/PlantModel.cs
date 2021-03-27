using ICSharpCode.NRefactory.Ast;
using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {
        public bool isEaten = false;
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
        }
        
        public PlantModel()
        {
            this.plantAge = 0;
            this.plantSize = 0;
        }
        
        
    }
    

}