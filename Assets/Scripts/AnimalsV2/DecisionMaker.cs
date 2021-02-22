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
        private Animal animal;
        private FiniteStateMachine fsm;
        
        // public List<GameObject> hostileTargets;
        // public List<GameObject> friendlyTargets;
        // public List<GameObject> foodTargets;

        public List<GameObject> seenTargets;
        public List<GameObject> heardTargets;

        public DecisionMaker(Animal animal, AnimalController animalController, AnimalModel animalModel,TickEventPublisher eventPublisher)
        {
            this.animal = animal;
            fsm = animal.Fsm;
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
            seenTargets = animalController.visibleTargets;
            heardTargets = animalController.heardTargets;
            List<GameObject> allTargets = seenTargets.Concat(heardTargets).ToList();
            Debug.Log(seenTargets.Count);
            Debug.Log(heardTargets.Count);
            
            bool predatorNearby = PredatorNearby(allTargets);
            bool foodNearby = FoodNearby(allTargets);

            
            //This is instead of using the state machine regularly.
            //SortedDictionary<State, List<State>> stateGraph = StateConstraintsGraph();
            
            //CHOOSE ACTION BASED ON PARAMETERS.
            //Parameters: objects with required traits


            //Preconditions: conditions on traits

            //Effect: change "state"/ delete object etc.

            Debug.Log("Get Best Action");
            Debug.Log(fsm.CurrentState.GetType());
            //No matter the current state, flee if getting eaten is iminent.
            //(fleeing is above and is therefore more prioritized)
            if (predatorNearby)
            {
                Debug.Log("RIP!");
                ChangeState(animal.fs);
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
            else if (currentState is SearchForFood)
            {
                SearchForFood searchForFood = (SearchForFood) currentState;
            
                if (searchForFood.adjacentToFood())
                {
                    //ChangeState(animal.es);
                }
            }
            else if (currentState is SearchForWater)
            {
                SearchForWater searchForWater = (SearchForWater) currentState;
                
                if (searchForWater.adjacentToWater())
                {
                    //TODO should be drinking
                    //ChangeState(animal.es);
                }
            }
            else if (currentState is SearchForMate)
            {
                SearchForMate searchForMate = (SearchForMate) currentState;
                if (searchForMate.adjacentToMate())
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
                ChangeState(animal.sw);
            }
            else if (lowEnergy()) //Prio 2 dont die from hunger -> Find Food.
            {
                ChangeState(animal.sf);
            }
            else if (highHydration() && highEnergy() && wantingOffspring()) // Prio 3 (If we live good) search for mate.
            {
                ChangeState(animal.sm);
            }
            else if (!highHydration() && highEnergy()) //Prio 4, not low hydration but not high either + high energy -> find Water.
            {
                ChangeState(animal.sw);
            }
            else if (highHydration() && !highEnergy()) //Prio 5, not low energy but not high either + high hydration -> find Food.
            {
                ChangeState(animal.sf);
            }
            else // dont know what to do? -> Idle.
            {
                ChangeState(animal.idle);
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
            return animalModel.currentEnergy > 250;
        }
        private bool lowEnergy()
        {
            return animalModel.currentEnergy < 100;
        }
        private bool hydrationFull()
        {
            return animalModel.hydration == animalModel.traits.maxHydration;
        }
        private bool highHydration()
        {
            return animalModel.hydration > 250;
        }
        private bool lowHydration()
        {
            return animalModel.hydration < 100;
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
            //
            // animalModel.actionPerceivedHostile += HandleHostileTarget;
            // animalModel.actionPerceivedFriendly += HandleFriendlyTarget;
            // animalModel.actionPerceivedFood += HandleFoodTarget;
        }
        
        
        private void EventUnsubscribe()
        {
            eventPublisher.onParamTickEvent -= MakeDecision;
            eventPublisher.onSenseTickEvent -= MakeDecision;
            //
            // animalModel.actionPerceivedHostile -= HandleHostileTarget;
            // animalModel.actionPerceivedFriendly -= HandleFriendlyTarget;
            // animalModel.actionPerceivedFood -= HandleFoodTarget;
        }

        /// <summary>
        /// Handle perceived target such that GetBestAction can then use it in deciding an action.
        /// </summary>
        /// <param name="target"> perceived target sent from either FieldOfView or HearingAbility,
        /// which method that will be called depends on the type of target </param>
        // private void HandleHostileTarget(GameObject target)
        // {
        //     //hostileTargets.Add(target);
        //     hostileTargets.Add(target);
        //     Debug.Log(target.name + " is hostile to " + animal.name);
        // }
        // private void HandleFriendlyTarget(GameObject target)
        // {
        //     friendlyTargets.Add(target);
        //     Debug.Log(target.name + " is a potential mate to " + animal.name);
        // }
        // private void HandleFoodTarget(GameObject target)
        // {
        //     foodTargets.Add(target);
        //     Debug.Log(target.name + " can be eaten by " + animal.name);
        // }



    }
}