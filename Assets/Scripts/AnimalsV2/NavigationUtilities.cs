/*
 * Author: Johan A
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AnimalsV2
{
    /// <summary>
    /// General purpose movement function. Add more general functions from states.  
    /// </summary>
    public static class NavigationUtilities
    {
        /// <summary>
        /// Calculates the point to run to given a character transform and target point to flee from.
        /// </summary>
        /// <param name="animalTransform">Transform from MonoBehavior. </param> 
        /// <param name="targetPoint"> Point to run from. </param>
        ///
        /// <returns></returns>
        public static Vector3 RunFromPoint(Transform animalTransform, Vector3 targetPoint)
        {
            Vector3 pointToAnimalVector;


            // Set direction

            pointToAnimalVector = animalTransform.position - targetPoint;

            return animalTransform.position + Vector3.Normalize(pointToAnimalVector);
        }

        public static void NavigateToPoint(AnimalController animal, Vector3 position)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, animal.agent.height * 2,
                1 << NavMesh.GetAreaFromName("Walkable")))
            {
                animal.agent.SetDestination(hit.position);
                
            }
            //TODO Maybe handle this!
            
            // NavMeshAgent agent = animal.agent;
            //     
            // if (Time.timeScale > 1.0f && agent.hasPath)
            // {
            //     agent.updatePosition = false;
            //     
            //     float maxAgentTravelDistance = Time.deltaTime * agent.speed;
            //
            //     //If at the end of path, stop agent.
            //     if (
            //         agent.SamplePathPosition(NavMesh.AllAreas, maxAgentTravelDistance, out hit) ||
            //         agent.remainingDistance <= agent.stoppingDistance
            //     ) {
            //         //Stop agent
            //     }
            //     //Else, move the actor and manually update the agent pos
            //     else {
            //         animal.transform.position = hit.position;
            //         agent.nextPosition = animal.transform.position;
            //     }
            // }
            
        }
        
        //https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        public static bool RandomPoint(Vector3 center, float range,float maxDist, out Vector3 result)
        {
            while (true)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, maxDist, 1 << NavMesh.GetAreaFromName("Walkable")))
                {
                    result = hit.position;
                    return true;

                    //Try opposite direction to avoid clinging to walls.
                }
            }
            //     else if (NavMesh.SamplePosition(new Vector3(-randomPoint.x, randomPoint.y, -randomPoint.z), out hit,
            //         maxDist, 1 << NavMesh.GetAreaFromName("Walkable")))
            //     {
            //         result = hit.position;
            //         return true;
            //     }
            // }
            
            
            result = center;
            return false;
        }
        
        /// <summary>
        /// Creates a perpendicular vector to a sampled position if possible.
        /// </summary>
        /// <param name="front"></param>
        /// <param name="up"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        
        public static bool PerpendicularPoint(Vector3 center,Vector3 front, Vector3 up,float maxDist, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 perpVec = center + Vector3.Cross(front,up).normalized;
                
                NavMeshHit hit;
                if (NavMesh.SamplePosition(perpVec, out hit, maxDist, 1 << NavMesh.GetAreaFromName("Walkable")))
                {
                    result = hit.position;
                    return true;
                    
                    //Otherwise try other direction
                }else if (NavMesh.SamplePosition(new Vector3(-perpVec.x,perpVec.y,-perpVec.z), out hit, maxDist, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Gives nearest object (of type with Tag in unity) to a given animal.
        /// </summary>
        /// <param name="a"> Animal to find objects near.</param>
        /// <param name="tag"> String tag to find objects of. </param>
        /// <returns> 3D position of the nearest object. </returns>
        public static GameObject GetNearestObject(List<GameObject> allPercievedObjects, Vector3 thisPosition)
        {
            //Return if not objects with tag found.
            if (allPercievedObjects == null || allPercievedObjects.Count == 0) return null;
            
            // Find closest object of all objects with tag
            GameObject nearbyObj = allPercievedObjects[0];
            float closestDistance = Mathf.Infinity;

            if (nearbyObj != null)
            {
                closestDistance = Vector3.Distance(nearbyObj.transform.position, thisPosition);


                //Get the closest game object
                foreach (GameObject g in allPercievedObjects)
                {
                    if (g != null)
                    {
                        float dist = Vector3.Distance(g.transform.position, thisPosition);
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            nearbyObj = g;
                        }
                    }
                }

                return nearbyObj ?? null;
            }

            return null;
        }

        /// <summary>
        /// This function could be extended upon to generate a better point.
        /// this would result in smarter fleeing behavior.
        /// </summary>
        /// <param name="a"> Animal to calculate positions from. </param>
        /// <returns> Vector3 </returns>
        public static Vector3 GetNearObjectsAveragePosition(List<GameObject> allPercievedObjects,
            Vector3 defaultPosition)
        {
            //Return if not objects with tag found.
            if (allPercievedObjects.Count == 0) return defaultPosition;

            Vector3 averagePosition = new Vector3();

            //Calculate the average
            foreach (GameObject g in allPercievedObjects)
            {
                if (g != null)
                {
                    averagePosition += g.transform.position;
                }
            }

            averagePosition /= allPercievedObjects.Count;
            return averagePosition;
        }

        // /// <summary>
        // /// Find all nearby objects of an animal.
        // /// </summary>
        // /// <param name="a">Animal whose nearby objects to find</param>
        // /// <param name="tag">Tag of the objects to find</param>
        // /// <returns>A list of all nearby objects to the animal with specified tag.</returns>
        // private static List<GameObject> GetAllPercievedObjectsWithTag(AnimalController a, string tag)
        // {
        //     // Find all nearby object of given type (Tag in Unity)
        //     List<GameObject> allPercievedObjects = a.heardTargets.Concat(a.visibleTargets).ToList();
        //     List<GameObject> allPercievedObjectsWithTag = new List<GameObject>();
        //     foreach (var o in allPercievedObjects)
        //     {
        //         if (o.tag.Equals(tag)) allPercievedObjectsWithTag.Add(o);
        //     }
        //     return allPercievedObjectsWithTag;
        // }

        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;

            randDirection += origin;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

            return navHit.position;
        }

        public static void NavigateRelative(AnimalController animal, Vector3 relativeVector, int layerMask)
        {
            //if the relative vector (which we want to navigate through) is zero, we return. Alos if animal is null we return
            if (relativeVector.Equals(Vector3.zero) || !animal)
            {
                return;
            }

            Vector3 origin = animal.transform.position;
            Vector3 destination = origin + relativeVector;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, Vector3.Distance(origin, relativeVector), layerMask) && !animal.agent.isStopped)
            {
                animal.agent.SetDestination(destination);
            }

        }
    }
}