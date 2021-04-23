using DataCollection;
using Model;

namespace ViewController
{
    public class BasicPlantController : PlantController
    {
        public override void onObjectSpawn()
        {
            plantModel = new PlantModel(35f, 40);
        }

        public override float GetEaten()
        {
            float nutrition = plantModel.GetEaten();
            if (!FindObjectOfType<ObjectPooler>())
            {
                Destroy(gameObject);
            }
            onDeadPlant?.Invoke(this);
            return nutrition;
        }
        
        
        public override string GetObjectLabel()
        {
            return "BasicPlant";
        }
    }
}