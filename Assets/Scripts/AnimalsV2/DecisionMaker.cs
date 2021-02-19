using System.Collections.Generic;
using System.Linq;
using AnimalsV2;
using AnimalsV2.States;
using UnityEditor;
using UnityEngine;

namespace AnimalsV2
{
    
    

    public class DecisionMaker : MonoBehaviour
    {
        private AnimalController animalController;
        private AnimalModel animalModel;
        private Animal animal;
        private FiniteStateMachine fsm;

        public DecisionMaker(Animal animal, AnimalController animalController, AnimalModel animalModel)
        {
            this.animal = animal;
            fsm = animal.Fsm;
            this.animalController = animalController;
            this.animalModel = animalModel;
        }

        public void Start()
        {
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
            //TODO This should have been accessable from model??!!
            List<GameObject> heardTargets = animalController.heardTargets;
            List<GameObject> seenTargets = animalController.visibleTargets;
            List<GameObject> allTargets = heardTargets.Concat(seenTargets).ToList();

            
            bool predatorNearby = PredatorNearby(allTargets);
            bool foodNearby = FoodNearby(seenTargets);

            
            //This is instead of using the state machine regularly.
            //SortedDictionary<State, List<State>> stateGraph = StateConstraintsGraph();
            
            //CHOOSE ACTION BASED ON PARAMETERS.
            //Parameters: objects with required traits


            //Preconditions: conditions on traits

            //Effect: change "state"/ delete object etc.
            
            //No matter the current state, flee if getting eaten is iminent.
            if (predatorNearby)
            {
                ChangeState(animal.fs);
                return;
            }


            State currentState = fsm.CurrentState;
            
            //TODO getType should be replaced with ID or similar. THIS IS REALLY BAD PRACTICE I KNOW.
            if (currentState is Eating)
            {
                Eating eatingState = (Eating) currentState;
                //Eat until full or out of food.
                if (eatingState.foodIsEmpty() || animalModel.currentEnergy == animalModel.traits.maxEnergy)
                {
                    ChangeState(animal.idle);
                }

            }else if (currentState is FleeingState)
            {
                FleeingState fleeingState = (FleeingState) currentState;
                //Run until no predator nearby.
                //Run a bit longer?
                
                //if we arrive here predatorNearby is false.
                ChangeState(animal.idle);
                
            }
            else if (currentState is Idle)
            {
                
                
            }
            else if (currentState is SearchForFood)
            {
                SearchForFood searchForFood = (SearchForFood) currentState;

                if (searchForFood.adjacentToFood())
                {
                    ChangeState(animal.es);
                }
            }
            else if (currentState is SearchForWater)
            {
                SearchForWater searchForWater = (SearchForWater) currentState;
                
            }
            else if (currentState is SearchForMate)
            {
                SearchForMate searchForMate = (SearchForMate) currentState;
            }

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
        
        private bool lowEnergy()
        {
            return animalModel.currentEnergy < 100;
        }
        private bool thirsty()
        {
            return animalModel.hydration < 100;
        }
        private bool wantingOffspring()
        {
            return animalModel.reproductiveUrge > 30;
        }
        private bool lowHealth()
        {
            return animalModel.currentHealth < 30;
        }

        //Instead of updating/Making choices every frame
        //Listen to when parameters or senses were updated.
        private void EventSubscribe()
        {
            FindObjectOfType<global::TickEventPublisher>().onParamTickEvent += MakeDecision;
            FindObjectOfType<global::TickEventPublisher>().onSenseTickEvent += MakeDecision;
        }
        

        private void EventUnsubscribe()
        {
            FindObjectOfType<global::TickEventPublisher>().onParamTickEvent -= MakeDecision;
            FindObjectOfType<global::TickEventPublisher>().onSenseTickEvent -= MakeDecision;
        
        }
        
        
        

    }
}