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
    }
}