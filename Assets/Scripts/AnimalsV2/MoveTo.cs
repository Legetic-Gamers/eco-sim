using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using FSM;

public class MoveTo : MonoBehaviour {
       
    NavMeshAgent agent;
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.destination = Random.insideUnitCircle * 20;
    }

    void Update () {
        if (agent.remainingDistance < 1.0f){
            agent.destination = Random.insideUnitCircle * 20;
        }
        }
}