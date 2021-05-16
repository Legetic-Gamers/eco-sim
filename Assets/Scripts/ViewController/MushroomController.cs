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

    public Material ripeMaterial;
    public Material youngMaterial;
    
    
    private float reproductionProbability = 0.05f;
    
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
        // float normalizedValue = 1f / PlantModel.plantMaxNutrition;
        // gameObject.transform.localScale = new Vector3(normalizedValue + 0.1f, normalizedValue + 0.1f,normalizedValue + 0.1f) * plantModel.nutritionValue;
        //   
        float nutritionPercent = (plantModel.nutritionValue / plantModel.plantMaxNutrition);
        gameObject.transform.localScale = new Vector3(nutritionPercent, nutritionPercent, nutritionPercent ) * 3f;
        
        //Update mushroom material to reflect maturity

        
            if (plantModel.isMature && ripeMaterial != null)
            {
                // renderer.material = renderer.materials[0];
                meshRenderer.material = ripeMaterial;
            }
            else if(youngMaterial != null)
            {
                // renderer.material = renderer.materials[1];
                meshRenderer.material = youngMaterial;
            }

            
        

    }
    
    private void Grow()
    {
        //it is not realistic that the plant increases in age and nutritionvalue while being invisible and suddenly pops up with a high nutrition and age (which directly depends on the 10s waitforseconds)
        if (plantModel.isRegrowing) return;
        
        //grow the plantmodel to increase age and nutritionvalue
        plantModel.Grow();
        
        SetPhenotype();

        float r = Random.Range(0, 1f);
        
        
        //0.9975
        // chance of reproducing every 2 seconds if age and size restrictions are met.
        if (plantModel.isMature && r > (1-reproductionProbability))
        {
            Reproduce();
        }
    }

    private void Reproduce()
    {
        var position = gameObject.transform.position;
            
            
        /*
        bool isHit = false;
        float height = 0;
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
        
        
        //If there already is mushroom nearby, dont spawn new
        //This is a restriction!
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 7f);
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.gameObject == gameObject) continue;
            
            if (hitCollider.gameObject.CompareTag("Plant"))
            {
                return;
            }

        }
            
        float rx = Random.Range(-10f, 10f);
        float rz = Random.Range(-10f, 10f);
        Vector3 newPosition = new Vector3(position.x + rx, position.y, position.z + rz);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPosition, out hit, 10f,
            1 << NavMesh.GetAreaFromName("Walkable")))
        {
            SpawnNewPlant?.Invoke(GetObjectLabel(),hit.position);
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
        yield return new WaitForSeconds(10f);
        plantModel.isRegrowing = false; //reset isregrowing which is set to true when GetEaten() in plantmodel is called
        meshRenderer.enabled = true;
        capsuleCollider.enabled = true;
        SetPhenotype();
    }

    //is triggered by an action in plantModel instead of checking every tick
    private void HandleDeathStatus()
    {
        if (gameObject.activeSelf)
        {
            //50% to spawn new
            // float r = Random.Range(0, 1f);
            // if (r > 0.5)
            // {
            //     Reproduce();
            // }
            
            //Reproduce();
            onDeadPlant?.Invoke(this);
            StopAllCoroutines();
            plantModel.onGrowOld -= HandleDeathStatus;
        }
    }
    
    public override void onObjectSpawn()
    {
        plantModel = new PlantModel();
        plantModel.nutritionValue = 0;
        plantModel.plantAge = 0;
        plantModel.onGrowOld += HandleDeathStatus;
        SetPhenotype();
        StartCoroutine(PlantControllerUpdate());
    }
    
    private IEnumerator PlantControllerUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 1.5f));
            Grow();
        }
    }
    
    public override string GetObjectLabel()
    {
        return "Mushroom";
    }
    
}
    

