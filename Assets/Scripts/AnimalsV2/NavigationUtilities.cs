/*
 * Author: Johan A
 */

using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <returns> 3D position of the nearest object. </returns>
        public static Vector3 GetNearestObjectPositionByTag(Animal a, String tag)
        {
            var allPercievedObjectsWithTag = GetAllPercievedObjectsWithTag(a, tag);


            Vector3 animalPosition = a.transform.position;
            //Return if not objects with tag found.
            if (allPercievedObjectsWithTag.Count == 0) return animalPosition;
            
            // Find closest object of all objects with tag
            Vector3 nearbyObj = allPercievedObjectsWithTag[0].transform.position;
            float closestDistance = Vector3.Distance(nearbyObj, animalPosition);
            //Get the closest game object
            foreach (GameObject g in allPercievedObjectsWithTag)
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
        
        /// <summary>
        /// This function could be extended upon to generate a better point.
        /// this would result in smarter fleeing behavior.
        /// </summary>
        /// <param name="a"> Animal to calculate positions from. </param>
        /// <returns></returns>
        public static Vector3 GetNearObjectsAveragePositionByTag(Animal a, String tag)
        {
            var allPercievedObjectsWithTag = GetAllPercievedObjectsWithTag(a, tag);
            
            Vector3 animalPosition = a.transform.position;
            //Return if not objects with tag found.
            if (allPercievedObjectsWithTag.Count == 0) return animalPosition;
            
            Vector3 averagePosition = new Vector3();
            
            //Calculate the average
            foreach (GameObject g in allPercievedObjectsWithTag)  averagePosition += g.transform.position;
            averagePosition /= a.heardTargets.Count;
            return averagePosition;
        }

        /// <summary>
        /// Find all nearby objects of an animal.
        /// </summary>
        /// <param name="a">Animal whose nearby objects to find</param>
        /// <param name="tag">Tag of the objects to find</param>
        /// <returns>A list of all nearby objects to the animal with specified tag.</returns>
        private static List<GameObject> GetAllPercievedObjectsWithTag(Animal a, string tag)
        {
            // Find all nearby object of given type (Tag in Unity)
            List<GameObject> allPercievedObjects = a.heardTargets.Concat(a.visibleTargets).ToList();
            List<GameObject> allPercievedObjectsWithTag = new List<GameObject>();
            foreach (var o in allPercievedObjects)
            {
                if (o.tag.Equals(tag))
                {
                    allPercievedObjectsWithTag.Add(o);
                }
            }

            return allPercievedObjectsWithTag;
        }

       
    }
}