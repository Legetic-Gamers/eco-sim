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
            onDeadPlant?.Invoke(this);
            return nutrition;
        }
    }
}