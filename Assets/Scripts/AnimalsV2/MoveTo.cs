using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using FSM;

public class MoveTo : MonoBehaviour {
       
    public Transform target;
    Vector3 destination;
    NavMeshAgent agent;
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        destination = agent.destination;
        //goal = Random.insideUnitCircle * 5;
    }

    void Update () {
        if (Vector3.Distance(destination, target.position) > 1.0f){
            destination = target.position;
            agent.destination = destination;
        }
        }
}