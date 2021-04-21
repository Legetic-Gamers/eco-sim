using System;
using System.Collections;
using DataCollection;
using Model;
using UnityEngine;
using UnityEngine.AI;
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

    private void SetPhenotype()
    {
        float normalizedValue = 1f / PlantModel.plantMaxsize;
        gameObject.transform.localScale = new Vector3(normalizedValue + 0.1f, normalizedValue + 0.1f,normalizedValue + 0.1f) * plantModel.nutritionValue;
    }
    
    private void Grow()
    {
        //it is not realistic that the plant increases in age and nutritionvalue while being invisible and suddenly pops up with a high nutrition and age (which directly depends on the 10s waitforseconds)
        if (plantModel.isRegrowing) return;
        
        //grow the plantmodel to increase age and nutritionvalue
        plantModel.Grow();
        
        SetPhenotype();

        float r = Random.Range(0, 1f);
        float rx = Random.Range(-10f, 10f);
        float rz = Random.Range(-10f, 10f);
        
        // chance of reproducing every 2 seconds if age and size restrictions are met.
        if (plantModel.plantAge > 20 && plantModel.nutritionValue > 30 && !plantModel.isRegrowing && r > 0.97)
        {
            float height = 0;
            bool isHit = false;
            
            var position = gameObject.transform.position;
            
            
            /*
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
            */
            
            Vector3 newPosition = new Vector3(position.x + rx, position.y, position.z + rz);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(newPosition, out hit, 1f,
                1 << NavMesh.GetAreaFromName("Walkable")))
            {
                SpawnNewPlant?.Invoke(GetObjectLabel(),hit.position);
            }

        }
    }
    
    public override float GetEaten()
    {
        if (gameObject.activeInHierarchy && !plantModel.isRegrowing)
        {
            StartCoroutine(Regrow());
        }
        return plantModel.GetEaten();
    }
    
    private IEnumerator Regrow()
    {
        meshRenderer.enabled = false;
        capsuleCollider.enabled = false;
        //dh.LogDeadPlant();
        yield return new WaitForSeconds(10f / Time.timeScale);
        plantModel.isRegrowing = false; //reset isregrowing which is set to true when GetEaten() in plantmodel is called
        meshRenderer.enabled = true;
        capsuleCollider.enabled = true;
        SetPhenotype();
        //dh.LogNewPlant();
    }

    //is triggered by an action in plantModel instead of checking every tick
    private void HandleDeathStatus()
    {
        
        if (gameObject.activeSelf)
        {
            //dh.LogDeadPlant();
            onDeadPlant?.Invoke(this);
            StopAllCoroutines();
        }
    }
    
    public override void onObjectSpawn()
    {
        plantModel = new PlantModel();
        plantModel._nutritionValue = 0;
        plantModel._plantAge = 0;
        plantModel.onGrowOld += HandleDeathStatus;
        SetPhenotype();
        //dh = FindObjectOfType<DataHandler>();
        //dh.LogNewPlant();
        
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
    
    public override string GetObjectLabel()
    {
        return "Mushroom";
    }
    
}
    

