/*
 * Author: Alexander L.V
 */

using System.Collections.Generic;
using System.Linq;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using UnityEngine;
using static AnimalsV2.Priorities;

namespace AnimalsV2
{
    public class DecisionMaker
    {
        //TODO REDUCE DEPENDENCIES.
        private AnimalController animalController;
        private AnimalModel animalModel;
        private TickEventPublisher eventPublisher;
        private FiniteStateMachine fsm;
        
        public DecisionMaker(AnimalController animalController, AnimalModel animalModel,TickEventPublisher eventPublisher)
        {
            
            fsm = animalController.Fsm;
            this.animalController = animalController;
            this.animalModel = animalModel;
            this.eventPublisher = eventPublisher;

            EventSubscribe();
        }
        
        /// <summary>
        /// Makes a decision based on the senses and the parameters of the animal (its perception of itself and its environment)
        /// to make a decision of an action to take.
        /// This action will most likely reslult in a new state as parameters/location/environment will have changed
        /// du to the action.
        /// 
        /// </summary>
        /// <returns></returns>
        private void MakeDecision()
        {//TODO STATE should be called ACTION instead?!
            GetBestAction(animalModel);
        }

        private void GetBestAction(AnimalModel parameters)
        {
            //This is instead of using the state machine regularly.
            //SortedDictionary<State, List<State>> stateGraph = StateConstraintsGraph();
            
            //CHOOSE ACTION BASED ON PARAMETERS.
            //Parameters: objects with required traits


            //Preconditions: conditions on traits

            //Effect: change "state"/ delete object etc.

            //Debug.Log("Get Best Action");
            //Debug.Log(fsm.CurrentState.GetType());
            //No matter the current state, flee if getting eaten is iminent.
            //(fleeing is above and is therefore more prioritized)
            // if (predatorNearby)
            // {
            //     ChangeState(animalController.fs);
            // }

            State currentState = fsm.CurrentState;

            switch (currentState)
            {
                case Eating state:
                {
                    Eating eatingState = state;
                    //Eat until full or out of food.
                    if (eatingState.foodIsEmpty() || animalModel.EnergyFull)
                    {
                        Prioritize();
                    }

                    break;
                }
                case Drinking state:
                {
                    Drinking drinkingState = state;
                    Prioritize();
                    break;
                }
                case FleeingState state:
                {
                    //Run until no predator nearby.
                    //Run a bit longer?

                    FleeingState fleeingState = state;
                    if (fleeingState.HasFled())
                    {
                        //if we arrive here there the animal has sucessfully fled
                        Prioritize();    
                    }

                    break;
                }
                case Idle _:
                    Prioritize();
                    break;
                case GoToState state:
                {
                    GoToState goToState = state;
                    if(goToState.GetTarget() != null && goToState.GetAction() != null){
                        if (goToState.arrivedAtTarget())
                        {
                            GameObject target = goToState.GetTarget();
                            Priorities actionToDo = goToState.GetAction();

                            switch (actionToDo)
                            {
                                case Food:
                                    animalController.eatingState.EatFood(target);
                                    ChangeState(animalController.eatingState);
                                    break;
                                case Water:
                                    animalController.drinkingState.DrinkWater(target);
                                    ChangeState(animalController.drinkingState);
                                    break;
                                case Mate:
                                    animalController.matingState.Mate(target);
                                    ChangeState(animalController.drinkingState);
                                    break;
                            }
                        
                        }
                    }
                    else
                    {
                        Prioritize();
                    }

                    break;
                }
                case Wander state:
                {
                    Wander wander = state;
                    var targetAndAction = wander.FoundObject();
                    if (targetAndAction?.Item1 != null)
                    {
                        animalController.goToState.SetTarget(targetAndAction.Item1);
                        animalController.goToState.SetActionOnArrive(targetAndAction.Item2);
                        ChangeState(animalController.goToState);
                    }
                    else
                    {
                        //If no food found, try to reprioritize the search.
                        Prioritize();
                    }

                    break;
                }
                case Mating _:
                    Prioritize();
                    break;
            }
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

        /// <summary>
        /// Represents the actions that can be transitioned between.
        /// </summary>
        /// <returns></returns>
        //TODO this could be represented in the states themselves, where each state holds a list of outgoing edges.
        // private static SortedDictionary<State, List<State>> StateConstraintsGraph()
        // {
        //     SortedDictionary<State, List<State>> stateGraph = new SortedDictionary<State, List<State>>();
        //     Eating eating = new Eating();
        //
        //     FleeingState fleeingState = new FleeingState();
        //     Idle idle = new Idle();
        //     SearchForFood searchForFood = new SearchForFood();
        //     SearchForMate searchForMate = new SearchForMate();
        //     SearchForWater searchForWater = new SearchForWater();
        //
        //
        //     
        //     
        //     return 
        // }


        //Instead of updating/Making choices every frame
        //Listen to when parameters or senses were updated.
        private void EventSubscribe()
        {
            eventPublisher.onParamTickEvent += MakeDecision;
            eventPublisher.onSenseTickEvent += MakeDecision;
            
            animalController.actionPerceivedHostile += HandleHostileTarget;
            animalController.actionDeath += HandleDeath;
        }
        
        
        public void EventUnsubscribe()
        {
            eventPublisher.onParamTickEvent -= MakeDecision;
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
            
            //Debug.Log(target.name + " is hostile to " + animalController.name);
        }

        private void HandleDeath()
        {
            ChangeState(animalController.deadState);
        }



    }
}