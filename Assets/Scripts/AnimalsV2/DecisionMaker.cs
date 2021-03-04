/*
 * Author: Alexander L.V
 */

using System;
using System.Collections.Generic;
using System.Linq;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using UnityEngine;
using static AnimalsV2.Priorities;

namespace AnimalsV2
{
    public class DecisionMaker : MonoBehaviour
    {
        //TODO REDUCE DEPENDENCIES.
        private AnimalController animalController;
        private AnimalModel animalModel;
        private TickEventPublisher eventPublisher;
        private FiniteStateMachine fsm;
        

        public void Start()
        {
            animalController = GetComponent<AnimalController>();
            fsm = animalController.fsm;
            animalModel = animalController.animalModel;
            eventPublisher = FindObjectOfType<global::TickEventPublisher>();
            EventSubscribe();
        }


        private void MakeDecision()
        {
            // no decision making while fleeing!
            if (fsm.CurrentState is FleeingState) return;

                if (animalModel.LowHydration) //Prio 1 don't die from dehydration -> Find Water.
            {
                ChangeState(animalController.goToWaterState); 
                return;
            }
            if (animalModel.LowEnergy) //Prio 2 dont die from hunger -> Find Food.
            {
                ChangeState(animalController.goToFoodState);        
                return;
            }

            if (!animalModel.HighHydration && !animalModel.HighEnergy) //Prio 6, not low energy but not high either + not low hydration but not high either -> find Water and then Food.
            {
                ChangeState(animalController.goToWaterState);
                return;
                
            }
            if (animalModel.HighHydration && !animalModel.HighEnergy) //Prio 5, not low energy but not high either + high hydration -> find Food.
            {
                ChangeState(animalController.goToFoodState);
                return;
            }
            
            if (!animalModel.HighHydration && animalModel.HighEnergy) //Prio 4, not low hydration but not high either + high energy -> find Water.
            {
                ChangeState(animalController.goToWaterState);
                return;
            }
            
            if (animalModel.LowEnergy) //Prio 2 dont die from hunger -> Find Food.
            {
                ChangeState(animalController.goToFoodState);
                return;
            }
            
            if (animalModel.LowHydration) //Prio 1 don't die from dehydration -> Find Water.
            {
                ChangeState(animalController.goToWaterState);
                return;
            }
            
            if (animalModel.WantingOffspring) // Prio 3 (If we live good) search for mate.
            {
                ChangeState(animalController.matingState);
                return;

            }
            ChangeState(animalController.wanderState);

        }

        /// <summary>
        /// Considers the animals internal state and depending on it chooses the next action.
        ///
        /// </summary>
        private void Prioritize()
        {
            List<Priorities> prio = new List<Priorities>();
            //Debug.Log("Prio!");
            
            if (animalModel.LowHydration) //Prio 1 don't die from dehydration -> Find Water.
            {
                prio.Add(Water);
                
            }
            if (animalModel.LowEnergy) //Prio 2 dont die from hunger -> Find Food.
            {
                prio.Add(Food);
                
            }

            if (!animalModel.HighHydration && !animalModel.HighEnergy) //Prio 6, not low energy but not high either + not low hydration but not high either -> find Water and then Food.
            {
                //ChangeState(animal.sf);
                prio.Remove(Food);
                prio.Remove(Water);
                
                prio.Insert(0, Food);
                prio.Insert(0, Water);
                
            }
            if (animalModel.HighHydration && !animalModel.HighEnergy) //Prio 5, not low energy but not high either + high hydration -> find Food.
            {
                //ChangeState(animal.sf);
                prio.Remove(Food);
                prio.Insert(0, Food);
                
            }
            
            if (!animalModel.HighHydration && animalModel.HighEnergy) //Prio 4, not low hydration but not high either + high energy -> find Water.
            {
                //ChangeState(animal.sw);
                prio.Remove(Water);
                prio.Insert(0,Water);
                
            }
            
            if (animalModel.LowEnergy) //Prio 2 dont die from hunger -> Find Food.
            {
                prio.Remove(Food);
                prio.Insert(0, Food);
                
            }
            
            if (animalModel.LowHydration) //Prio 1 don't die from dehydration -> Find Water.
            {
                prio.Remove(Water);
                prio.Insert(0,Water);
                
            }

            if (animalModel.WantingOffspring) // Prio 3 (If we live good) search for mate.
            {
                prio.Insert(0,Mate);
                
            }
            
            //prio.Add("Mate");
            
            animalController.wanderState.SetPriorities(prio);
            ChangeState(animalController.wanderState);

            //Debug.Log(fsm.CurrentState.GetType());
        }
        
        private void ChangeState(State newState)
        {
            fsm.ChangeState(newState);
        }
        


        //Instead of updating/Making choices every frame
        //Listen to when parameters or senses were updated.
        private void EventSubscribe()
        {
            //eventPublisher.onParamTickEvent += MakeDecision;
            eventPublisher.onSenseTickEvent += MakeDecision;
            
            animalController.actionPerceivedHostile += HandleHostileTarget;
            animalController.actionDeath += HandleDeath;
        }
        
        
        public void EventUnsubscribe()
        {
            //eventPublisher.onParamTickEvent -= MakeDecision;
            eventPublisher.onSenseTickEvent -= MakeDecision;
            
            animalController.actionPerceivedHostile -= HandleHostileTarget;
            animalController.actionDeath -= HandleDeath;
        }

        /// <summary>
        /// Handle perceived target such that GetBestAction can then use it in deciding an action.
        /// </summary>
        /// <param name="target"> perceived target sent from either FieldOfView or HearingAbility,
        /// which method that will be called depends on the type of target </param>
        private void HandleHostileTarget(GameObject target)
        {
            ChangeState(animalController.fleeingState);
        }

        private void HandleDeath()
        {
            ChangeState(animalController.deadState);
        }


    }
}
