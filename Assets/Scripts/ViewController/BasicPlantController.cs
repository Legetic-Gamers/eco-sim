using Model;

namespace ViewController
{
    public class BasicPlantController : PlantController
    {

        private void Start()
        {
            plantModel = new PlantModel(35f);
        }
        public override float GetEaten()
        {
            float nutrition = plantModel.GetEaten();
            Destroy(gameObject);
            return nutrition;
        }
    }
}