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
        /// Calculates the point to run to given a character transform and target point to flee from or run towards.
        /// </summary>
        /// <param name="animalTransform">Transform from MonoBehavior. </param> 
        /// <param name="targetPoint"> Point to run from or to. </param>
        /// <param name="toPoint">True: Run towards point, False: Run away from point</param>
        /// <returns></returns>
        public static Vector3 RunToFromPoint(Transform animalTransform, Vector3 targetPoint,bool toPoint)
        {
            Vector3 pointToAnimalVector;
            
            
            // Set direction
            if (toPoint) pointToAnimalVector = targetPoint - animalTransform.position;
            else pointToAnimalVector = animalTransform.position - targetPoint;

            return animalTransform.position + Vector3.Normalize(pointToAnimalVector);
        }

        public static void NavigateToPoint(AnimalController animal,Vector3 position)
        {
            
            NavMeshHit hit;
            //TODO this maxDistance is what is causing rabbits to dance sometimes, if poisition cant be found.
            if (NavMesh.SamplePosition(position, out hit, animal.agent.height * 2, 1 << NavMesh.GetAreaFromName("Walkable")))
            {
                animal.agent.SetDestination(hit.position);
            }
            else
            {
                Debug.Log("NOOO");
            }
            
        }

        /// <summary>
        /// Gives nearest object (of type with Tag in unity) to a given animal.
        /// </summary>
        /// <param name="a"> Animal to find objects near.</param>
        /// <param name="tag"> String tag to find objects of. </param>
        /// <returns> 3D position of the nearest object. </returns>
        public static GameObject GetNearestObject([CanBeNull] List<GameObject> allPercievedObjects, Vector3 thisPosition)
        {
            if(allPercievedObjects == null)
                Debug.Log("IS NULL");
            
            //Return if not objects with tag found.
            if (allPercievedObjects == null || allPercievedObjects.Count == 0) return null;
            
            // Find closest object of all objects with tag
            GameObject nearbyObj = allPercievedObjects[0];

            if (nearbyObj != null)
            {
                float closestDistance = Vector3.Distance(nearbyObj.transform.position, thisPosition);
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
                return nearbyObj;
            }
            //This return was weird for me since it would be logical to return nearbyObj either if its null or not, but otherwise i get "Gameobject has been destroyed but you are still trying to access it
            return null;
        }
        
        /// <summary>
        /// This function could be extended upon to generate a better point.
        /// this would result in smarter fleeing behavior.
        /// </summary>
        /// <param name="a"> Animal to calculate positions from. </param>
        /// <returns> Vector3 </returns>
        public static Vector3 GetNearObjectsAveragePosition(List<GameObject> allPercievedObjects,Vector3 defaultPosition)
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

        public static bool NavigateRelative(AnimalController animal, Vector3 relativeVector, int layerMask)
        {
            //if the relative vector (which we want to navigate through) is zero, we return. Alos if animal is null we return
            if (relativeVector.Equals(Vector3.zero) || !animal)
            {
                return true;
            }

            Vector3 origin = animal.transform.position;
            Vector3 destination = origin + relativeVector;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, Vector3.Distance(origin, relativeVector), layerMask))
            {
                animal.agent.SetDestination(destination);
                return true;
            }
            
            return false;
            
        }
        
    }
}