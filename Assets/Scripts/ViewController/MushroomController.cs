using DataCollection;
using Model;
using UnityEngine;
using ViewController;

public class MushroomController : PlantController
{
    
    private TickEventPublisher tickEventPublisher;
    
    private DataHandler dh;
    
    public void Start()
    {
        tickEventPublisher = FindObjectOfType<TickEventPublisher>();
        plantModel = new PlantModel();
        EventSubscribe();

        dh = FindObjectOfType<DataHandler>();
        dh?.LogNewPlant(plantModel);
    }
    

    public void OnDestroy()
    {
        if (tickEventPublisher)
        {
            tickEventPublisher.onParamTickEvent -= HandleDeathStatus;
            tickEventPublisher.onParamTickEvent -= Grow;
        }
    }

    private void EventSubscribe()
    {
        if (tickEventPublisher)
        {
            tickEventPublisher.onParamTickEvent += HandleDeathStatus;  
            tickEventPublisher.onParamTickEvent += Grow;  
        }
    }
    
    private void EventUnSubscribe()
    {
        if (tickEventPublisher)
        {
            tickEventPublisher.onParamTickEvent -= HandleDeathStatus;  
            tickEventPublisher.onParamTickEvent -= Grow;  
        }
    }
    
    private void SetPhenotype()
    {
        float normalizedValue = 1f / PlantModel.plantMaxsize;
        gameObject.transform.localScale = new Vector3(normalizedValue, normalizedValue,normalizedValue) * plantModel.nutritionValue;
    }
    
    private void Grow()
    {
        
        plantModel.plantAge += 2;
        if (plantModel.nutritionValue > PlantModel.plantMaxsize)
        {
            plantModel.nutritionValue = PlantModel.plantMaxsize;
            SetPhenotype();
        }
        else plantModel.nutritionValue += 2;

        // plant has regrown after being eaten
        if (plantModel.isEaten && plantModel.nutritionValue > 15)
        {
            gameObject.SetActive(true);
            SetPhenotype();
        }

        float r = Random.Range(0, 1f);
        float rx = Random.Range(-10f, 10f);
        float rz = Random.Range(-10f, 10f);
        // chance of reproducing every 2 seconds if age and size restrictions are met.
        //if (plantModel.nutritionValue > 15 && !plantModel.isEaten && r > 0.95) 
        if (plantModel.plantAge > 15 && plantModel.nutritionValue > 15 && !plantModel.isEaten && r > 0.95)
        {
            float height = 0;
            bool isHit = false;

            var position = gameObject.transform.position;
            Vector3 newPosition = new Vector3(position.x + rx, position.y + 100, position.z + rz);
            Ray ray = new Ray(newPosition, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    height = hit.point.y;
                    isHit = true;
                }
            }
            if (!isHit) return;
            GameObject offspring = Instantiate(gameObject);
            offspring.transform.position = new Vector3(position.x + rx, height, position.z + rz);
            PlantModel offspringModel = new PlantModel();
            offspring.GetComponent<PlantController>().plantModel = offspringModel;
        }
    }
    
    public override float GetEaten()
    {
        gameObject.SetActive(false);
        return plantModel.GetEaten();
    }


    private void HandleDeathStatus()
    {
        if (plantModel != null && plantModel.plantAge > PlantModel.plantMaxAge)
        {
            dh?.LogDeadPlant(plantModel);
            EventUnSubscribe();
            Destroy(gameObject);
        }
    }
}
    

