/*
 * Author: Alexander L.V
 */

using System.Collections.Generic;
using System.Linq;
using AnimalsV2.States;
using UnityEngine;

namespace AnimalsV2
{
    public class DecisionMaker
    {
        //TODO REDUCE DEPENDENCIES.
        private AnimalController animalController;
        private AnimalModel animalModel;
        private TickEventPublisher eventPublisher;
        private FiniteStateMachine fsm;
        
        // public List<GameObject> hostileTargets;
        // public List<GameObject> friendlyTargets;
        // public List<GameObject> foodTargets;

        public List<GameObject> seenTargets;
        public List<GameObject> heardTargets;

        public DecisionMaker(AnimalController animalController, AnimalModel animalModel,TickEventPublisher eventPublisher)
        {
            
            fsm = animalController.Fsm;
            this.animalController = animalController;
            this.animalModel = animalModel;
            this.eventPublisher = eventPublisher;

            //TESTING!!!!!!
            // hostileTargets = animal.heardTargets;
            // friendlyTargets = new List<GameObject>();
            // foodTargets = new List<GameObject>();
           
            
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
            List<GameObject> allHostileTargets = animalController.visibleHostileTargets
                .Concat(animalController.heardHostileTargets).ToList();
            List<GameObject> allPreyTargets = animalController.visiblePreyTargets
                .Concat(animalController.heardPreyTargets).ToList();
            
            // with allHostileTargets as a list only containing predators, can instead check that it is not empty
            bool predatorNearby = PredatorNearby(allHostileTargets);
            bool foodNearby = FoodNearby(allPreyTargets);

            
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
            if (predatorNearby)
            {
                //Debug.Log("RIP!");
                ChangeState(animalController.fs);
                return;
            }

            //TEST
            //ChangeState(animal.fs);

            State currentState = fsm.CurrentState;
            
            //TODO getType should be replaced with ID or similar. THIS IS REALLY BAD PRACTICE I KNOW.
            if (currentState is Eating)
            {
                Eating eatingState = (Eating) currentState;
                //Eat until full or out of food.
                if (eatingState.foodIsEmpty() || energyFull())
                {
                    Prioritize();
                }
            
            }else if (currentState is FleeingState && !predatorNearby)
            {
                //FleeingState fleeingState = (FleeingState) currentState;
                //Run until no predator nearby.
                //Run a bit longer?
                
                //if we arrive here predatorNearby is false.
                Prioritize();
                
            }
            else if (currentState is Idle)
            {
                
                Prioritize();
            }
            //Always finish eating/drinking/mating
            else if (currentState is GoToFood)
            {
                GoToFood goToFood = (GoToFood) currentState;
            
                if (goToFood.adjacentToFood())
                {
                    //ChangeState(animal.es);
                }
            }
            else if (currentState is GoToWater)
            {
                GoToWater goToWater = (GoToWater) currentState;
                
                if (goToWater.adjacentToWater())
                {
                    //TODO should be drinking
                    //ChangeState(animal.es);
                }
            }
            else if (currentState is GoToMate)
            {
                GoToMate goToMate = (GoToMate) currentState;
                if (goToMate.adjacentToMate())
                {
                    //TODO should be mating
                    //ChangeState(animal.es);
                }
            }

        }
        
        

        /// <summary>
        /// Considers the animals internal state and depending on it chooses the next action.
        ///
        /// </summary>
        private void Prioritize()
        {
            Debug.Log("Prio!");
            if (lowHydration()) //Prio 1 don't die from dehydration -> Find Water.
            {
                ChangeState(animalController.sw);
            }
            else if (lowEnergy()) //Prio 2 dont die from hunger -> Find Food.
            {
                ChangeState(animalController.sf);
            }
            else if (highHydration() && highEnergy() && wantingOffspring()) // Prio 3 (If we live good) search for mate.
            {
                //ChangeState(animal.sm);
                ChangeState(animalController.wander);
            }
            else if (!highHydration() && highEnergy()) //Prio 4, not low hydration but not high either + high energy -> find Water.
            {
                //ChangeState(animal.sw);
                ChangeState(animalController.wander);
            }
            else if (highHydration() && !highEnergy()) //Prio 5, not low energy but not high either + high hydration -> find Food.
            {
                //ChangeState(animal.sf);
                ChangeState(animalController.wander);
            }
            else // dont know what to do? -> Idle.
            {
                ChangeState(animalController.idle);
            }

            Debug.Log(fsm.CurrentState.GetType());
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

        private static bool FoodNearby(List<GameObject> seenTargets)
        {
            return seenTargets.Any(o => o.CompareTag("Food"));
        }

        private static bool PredatorNearby(List<GameObject> allTargets)
        {
            
            return allTargets.Any(o => o.CompareTag("Predator"));
        }
        
        private bool energyFull()
        {
            return animalModel.currentEnergy == animalModel.traits.maxEnergy;
        }
        
        private bool highEnergy()
        {
            return animalModel.currentEnergy > 20;
        }
        private bool lowEnergy()
        {
            return animalModel.currentEnergy < 10;
        }
        private bool hydrationFull()
        {
            return animalModel.hydration == animalModel.traits.maxHydration;
        }
        private bool highHydration()
        {
            return animalModel.hydration > 20;
        }
        private bool lowHydration()
        {
            return animalModel.hydration < 10;
        }
        private bool wantingOffspring()
        {
            //reproductive urge greater than average of energy and hydration.
            return animalModel.reproductiveUrge > (animalModel.currentEnergy + animalModel.hydration) / 2;
        }
        private bool lowHealth()
        {
            return animalModel.currentHealth < 30;
        }

        //Instead of updating/Making choices every frame
        //Listen to when parameters or senses were updated.
        private void EventSubscribe()
        {
            eventPublisher.onParamTickEvent += MakeDecision;
            eventPublisher.onSenseTickEvent += MakeDecision;
            
            animalModel.actionPerceivedHostile += HandleHostileTarget;
        }
        
        
        private void EventUnsubscribe()
        {
            eventPublisher.onParamTickEvent -= MakeDecision;
            eventPublisher.onSenseTickEvent -= MakeDecision;
            
            animalModel.actionPerceivedHostile -= HandleHostileTarget;
        }

        /// <summary>
        /// Handle perceived target such that GetBestAction can then use it in deciding an action.
        /// </summary>
        /// <param name="target"> perceived target sent from either FieldOfView or HearingAbility,
        /// which method that will be called depends on the type of target </param>
        private void HandleHostileTarget(GameObject target)
        {
            ChangeState(animalController.fs);
            
            // can do this instead of calling NavigationUtilities.GetNearestObjectPositionByTag
            animalController.fs.fleeingFromPos = target.transform.position;
            Debug.Log(target.name + " is hostile to " + animalController.name);
        }



    }
}