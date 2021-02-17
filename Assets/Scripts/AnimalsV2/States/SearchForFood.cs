using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEngine.AI;
using static FSM.StateAnimation;

public class SearchForFood : FSMState<Animal>
{
    static readonly SearchForFood instance = new SearchForFood();
    public static SearchForFood Instance {
        get {
            return instance;
        }
    }
    static SearchForFood() { }
    private SearchForFood() { }
    

    public override void Enter (Animal a) {
        Debug.Log("Search for food...");
        currentStateAnimation = Running;
        
    }

    public override void Execute (Animal a)
    {
        currentStateAnimation = Walking;
        //Get average position of enemies and run away from it.
        Vector3 foodPos = Utilities.GetNearest(a, "Food");
        Vector3 pointToRunTo = Utilities.RunToFromPoint(a.transform,foodPos,true);
        //Move the animal using the navmeshagent.
        NavMeshHit hit;
        NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
        a.nMAgent.SetDestination(hit.position);
    }

    private Vector3 GetNearestFood(Animal a)
    {   
        
        Vector3 animalPosition = a.transform.position;
        if (a.nearbyObjects.Length == 0) return animalPosition;
            
            
        Vector3 nearbyFoodPos = a.nearbyObjects[0].transform.position;
        float closestDistance = Vector3.Distance(nearbyFoodPos, animalPosition);
            
        foreach (GameObject g in a.nearbyObjects)
        {
            float dist = Vector3.Distance(g.transform.position, animalPosition);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearbyFoodPos = g.transform.position;
            }
                
        }
        return nearbyFoodPos;
    }

    public override void Exit(Animal a) {

    }
}