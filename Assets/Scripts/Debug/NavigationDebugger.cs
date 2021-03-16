using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class NavigationDebugger : MonoBehaviour
{
    
    private NavMeshAgent agentToDebug;
    private LineRenderer lineRenderer;
    
    
    // Start is called before the first frame update
    void Start()
    {
        agentToDebug = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (agentToDebug.hasPath)
        {
            lineRenderer.positionCount = agentToDebug.path.corners.Length;
            lineRenderer.SetPositions(agentToDebug.path.corners);
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
