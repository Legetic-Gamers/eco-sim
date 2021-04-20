using DataCollection;
using Model;

namespace ViewController
{
    public class BasicPlantController : PlantController
    {
        public override void onObjectSpawn()
        {
            plantModel = new PlantModel(35f);
            dh?.LogNewPlant();
        }

        public override float GetEaten()
        {
            float nutrition = plantModel.GetEaten();
            if (onDeadPlant != null)
            {
                onDeadPlant?.Invoke(this);
            }
            else
            {
                Destroy(gameObject);
            }
            return nutrition;
        }
    }
}