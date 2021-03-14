using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;


public class MoveToGoalAgent : Agent
{

    [SerializeField]
    private Transform targetTransform;

    //Just for visualization, not required.
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        
        //Reset stuff 
        //MAKE SURE YOU ARE USING LOCAL POSITION
        transform.localPosition = new Vector3(Random.Range(-4.5f,0f),0,Random.Range(-3f,1.3f));
        transform.localPosition = new Vector3(Random.Range(-4.5f,0f),0,Random.Range(-3f,1.3f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        base.OnActionReceived(vectorAction);

        float moveX = vectorAction[0];
        float moveZ = vectorAction[1];

        float moveSpeed = 10f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

    }

    //TESTING
    public override void Heuristic(float[] actionsOut)
    {
        base.Heuristic(actionsOut);
        float[] continousActions = actionsOut;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }

        
    }
}
