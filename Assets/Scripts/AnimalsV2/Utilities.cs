using System;
using AnimalsV2;
using UnityEngine;
using UnityEngine.AI;

namespace FSM
{
    public static class Utilities
    {
        //Uses
        //Inspired by: https://answers.unity.com/questions/868003/navmesh-flee-ai-flee-from-player.html 
        public static Vector3 RunToFromPoint(Transform animalTransform,Vector3 targetPoint,bool toPoint)
        {
            
            Vector3 pointToAnimalVector;

            //Run to point or from point
            if (toPoint)
            {
                pointToAnimalVector = targetPoint - animalTransform.position;
            }
            else
            {
                pointToAnimalVector = animalTransform.position - targetPoint;
            }

            //Calculate point to run towards or from.
            return animalTransform.position + Vector3.Normalize(pointToAnimalVector);
        }
        
        public static Vector3 GetNearest(Animal a, String tag)
        {
            a.nearbyObjects = GameObject.FindGameObjectsWithTag(tag);
            Vector3 animalPosition = a.transform.position;
            if (a.nearbyObjects.Length == 0) return animalPosition;
            
            
            Vector3 nearbyFoodPos = a.nearbyObjects[0].transform.position;
            float closestDistance = Vector3.Distance(nearbyFoodPos, animalPosition);
            
            foreach (GameObject g in a.nearbyObjects)
            {
                float dist = Vector3.Distance(g.transform.position, animalPosition);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    nearbyFoodPos = g.transform.position;
                }
                
            }
            return nearbyFoodPos;
        }
    }
}