using System;
using UnityEngine;
using UnityEngine.AI;

namespace Cameras
{
    
    public class ClickToNavigateAgent : MonoBehaviour
    {
        private NavMeshAgent agent;

        private Camera camera;

        private void Start()
        {
            camera = Camera.main;
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    agent.SetDestination(hit.point);
                }
            }   
        }
    }
}