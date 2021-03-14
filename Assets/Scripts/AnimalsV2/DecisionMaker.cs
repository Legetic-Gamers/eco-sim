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
            //TODO STATE should be called ACTION instead?!
            GetBestAction(animalModel);
        }

        private void GetBestAction(AnimalModel parameters)
        {
            // no decision making while fleeing!
            if (fsm.CurrentState is FleeingState || fsm.CurrentState is EatingState ||
                fsm.CurrentState is DrinkingState || fsm.CurrentState is MatingState) return;
            Prioritize();
        }

        /// <summary>
        /// Considers the animals internal state and depending on it chooses the next action.
        ///
        /// </summary>
        private void Prioritize()
        {
            List<Priorities> prio = new List<Priorities>();


            if (!animalModel.HighHydration && !animalModel.HighEnergy)
                //not low energy but not high either + not low hydration but not high either -> find Water and then Food.
            {
                prio.Remove(Food);
                prio.Remove(Water);

                prio.Insert(0, Food);
                prio.Insert(0, Water);
            }

            if (animalModel.HighHydration && !animalModel.HighEnergy)
                //not low energy but not high either + high hydration -> find Food.
            {
                prio.Remove(Food);
                prio.Insert(0, Food);
            }

            if (!animalModel.HighHydration && animalModel.HighEnergy)
                //not low hydration but not high either + high energy -> find Water.
            {
                prio.Remove(Water);
                prio.Insert(0, Water);
            }

            if (animalModel.LowEnergy)
                //dont die from hunger -> Find Food.
            {
                prio.Remove(Food);
                prio.Insert(0, Food);
            }

            if (animalModel.LowHydration)
                //don't die from dehydration -> Find Water.
            {
                prio.Remove(Water);
                prio.Insert(0, Water);
            }

            if (animalModel.WantingOffspring)
                // (If we live good) search for mate.
            {
                prio.Insert(0, Mate);
            }

            //TODO det som händer här är att det blir alltid den som är sist i priority vi går till, which is bad.
            foreach (var priority in prio)
            {
                switch (priority)
                {
                    case Food:
                        //Kolla if(we actually meet requirements? and then return?)
                        if (ChangeState(animalController.goToFoodState))
                        {
                            return;
                        }

                        break;
                    case Water:
                        if (ChangeState(animalController.goToWaterState))
                        {
                            return;
                        }
                        
                        break;
                    case Mate:
                        if (ChangeState(animalController.goToMate))
                        {
                            return;
                        }

                        break;
                    default:
                        fsm.GoToDefaultState();
                        break;
                }
            }
        }

        private bool ChangeState(State newState)
        {
            return fsm.ChangeState(newState);
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
            Debug.Log("You dead!");
            ChangeState(animalController.deadState);
        }
    }
}