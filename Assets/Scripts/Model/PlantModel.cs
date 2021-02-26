using UnityEngine;

namespace Model
{
    public class PlantModel : IEdible
    {
        public bool isEaten = false;
        public float GetEaten()
        {
            isEaten = true;
            return 30f;
        }
    }
}