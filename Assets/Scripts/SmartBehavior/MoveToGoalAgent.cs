using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;


public class MoveToGoalAgent : Agent
{
    public override void OnActionReceived(float[] vectorAction)
    {
        base.OnActionReceived(vectorAction);
        Debug.Log(vectorAction[0]);
    }
    
    
}
