using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class SmallWorld : World
{
    // Start is called before the first frame update
    public override GameObject SpawnNew(GameObject type)
    {
        GameObject g = Instantiate(type, new Vector3(Random.Range(-rangeX, rangeX), 0,
                Random.Range(-rangeZ, rangeZ)) + transform.position,
            Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f)));

        if (g.CompareTag("Water"))
        {
            g.transform.position = transform.position + new Vector3(Random.Range(-rangeX, rangeX), 0,
                Random.Range(0, rangeZ));
        }

        
        Agent agent = g.GetComponent<Agent>();
        if (agent)
        {
            if (agent is AnimalBrainAgent animalBrainAgent)
            {
                g.transform.position = transform.position + new Vector3(Random.Range(-rangeX, rangeX), 0,
                    Random.Range(-rangeZ/2, rangeZ));
            }
                
            
        }
        
        return g;
    }
}
