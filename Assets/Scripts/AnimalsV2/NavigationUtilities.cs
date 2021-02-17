/*
 * Author: Johan A
 */

using System;
using UnityEngine;

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
        
        /// <summary>
        /// Gives nearest object (of type with Tag in unity) to a given animal.
        /// </summary>
        /// <param name="a"> Animal to find objects near.</param>
        /// <param name="tag"> String tag to find objects of. </param>
        /// <returns> GameObject to be used by NavMeshAgent. </returns>
        public static Vector3 GetNearestObjectByTag(Animal a, String tag)
        {
            // Find all nearby object of given type (Tag in Unity)
            a.nearbyObjects = GameObject.FindGameObjectsWithTag(tag);
            Vector3 animalPosition = a.transform.position;
            
            if (a.nearbyObjects.Length == 0) return animalPosition;
            // Find closest object of all objects with tag
            Vector3 nearbyObj = a.nearbyObjects[0].transform.position;
            float closestDistance = Vector3.Distance(nearbyObj, animalPosition);
            foreach (GameObject g in a.nearbyObjects)
            {
                float dist = Vector3.Distance(g.transform.position, animalPosition);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    nearbyObj = g.transform.position;
                }
            }
            return nearbyObj;
        }
    }
}