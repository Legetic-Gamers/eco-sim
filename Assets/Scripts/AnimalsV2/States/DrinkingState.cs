
    /*
 * Authors: Alexander L.V, Johan A.
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AnimalsV2.States
{
    public class DrinkingState : State
    {
        private GameObject target;

        public Action<GameObject, float> onDrinkWater;
        
        
        public DrinkingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Attack;
            
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = true;
            }
            
            animal.StartCoroutine(DrinkWater());
            
            //GetNearestWater();
        }
        
        

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // if (MeetRequirements())
            // {
            //     
            //     DrinkWater(target);
            // }
            // else
            // {
            //     finiteStateMachine.GoToDefaultState();
            // }
        }

        public override void Exit()
        {
            base.Exit();
            
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = false;
            }
            
            animal.StopCoroutine(DrinkWater());
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        public IEnumerator DrinkWater()
        {
            
            // Wait a while then drink, change state and resume walking
            yield return new WaitForSeconds(1);

            
            onDrinkWater?.Invoke(target, animal.animalModel.currentHydration);
            
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = false;
            }
            Debug.Log("Succesfully drank.");

            finiteStateMachine.GoToDefaultState();

            // Very important, this tells Unity to move onto next frame. Everything crashes without this
            yield return null;
        }

        public override string ToString()
        {
            return "Drinking";
        }

        // public override bool MeetRequirements()
        // {
        //     Vector3 position = animal.transform.position;
        //     target = GetNearestWater();
        //     if (target == null) return false;
        //     bool isCloseEnough = Vector3.Distance(target.transform.position, position) <= 3f;
        //     return animal.visibleWaterTargets.Count > 0 && isCloseEnough;
        // }
        
        public override bool MeetRequirements()
        {
            return target != null && !animal.animalModel.HighHydration;
        }

        private GameObject GetNearestWater()
        {
            Vector3 position = animal.transform.position;
            return NavigationUtilities.GetNearestObject(animal.visibleWaterTargets, position);
        }
    }

}