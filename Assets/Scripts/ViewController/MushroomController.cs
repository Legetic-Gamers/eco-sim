using System;
using System.Collections;
using DataCollection;
using Model;
using UnityEngine;
using ViewController;
using Random = UnityEngine.Random;

public class MushroomController : PlantController
{
    public CapsuleCollider capsuleCollider;
    public MeshRenderer meshRenderer;

    public void Start()
    {
        //If there is no object pooler present, we need to call onObjectSpawn through start
        if (FindObjectOfType<ObjectPooler>() == null)
        {
            onObjectSpawn();
        }
    }

    private void EventSubscribe()
    {
        
    }
    
    private void EventUnSubscribe()
    {
        
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
        if(plantModel.plantAge > PlantModel.plantMaxAge) HandleDeathStatus();
        
        else plantModel.nutritionValue += 3f;

        float r = Random.Range(0, 1f);
        float rx = Random.Range(-10f, 10f);
        float rz = Random.Range(-10f, 10f);
        // chance of reproducing every 2 seconds if age and size restrictions are met.
        //if (plantModel.nutritionValue > 15 && !plantModel.isEaten && r > 0.95) 
        if (plantModel.plantAge > 15 && plantModel.nutritionValue > 30 && !plantModel.isEaten && r > 0.95)
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
            SpawnNewPlant?.Invoke(new Vector3(position.x + rx, height, position.z + rz));
        }
    }
    
    public override float GetEaten()
    {
        if (gameObject.activeInHierarchy && !plantModel.isRegrowing)
        {
            plantModel.isEaten = true;
            plantModel.isRegrowing = true;
            StartCoroutine(Regrow());
            plantModel.isEaten = false;
            plantModel.isRegrowing = false;
        }
        return plantModel.GetEaten();
    }
    
    private IEnumerator Regrow()
    {
        meshRenderer.enabled = false;
        capsuleCollider.enabled = false;
        dh.LogDeadPlant();
        yield return new WaitForSeconds(10f / Time.timeScale);
        meshRenderer.enabled = true;
        capsuleCollider.enabled = true;
        dh.LogNewPlant();
    }


    private void HandleDeathStatus()
    {
        if (gameObject.activeSelf && plantModel.plantAge > PlantModel.plantMaxAge && !plantModel.isEaten)
        {
            onDeadPlant?.Invoke(this);
            EventUnSubscribe();
        }
    }
    
    public override void onObjectSpawn()
    {
        plantModel = new PlantModel();
        
        dh = FindObjectOfType<DataHandler>();
        dh?.LogNewPlant();
        
        EventSubscribe();
        
        StartCoroutine(PlantControllerUpdate());
    }
    
    private IEnumerator PlantControllerUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 1.5f)/Time.timeScale);
            Grow();
        }
    }
}
    

