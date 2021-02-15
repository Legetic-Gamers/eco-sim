using UnityEngine;
using static FSM.StateAnimation;


//Author: Alexander LV
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/

//Also, Singleton pattern: https://csharpindepth.com/articles/singleton

namespace FSM
{
    
    
//State where the animal searches actively for a mate.
//sealed just prevents other classes from inheriting. Not necessary for pattern since private constructor already makes sure of this.
//However might improve optimization.
    public sealed class SearchForMate : FSMState<Animal>
    {
        private static readonly SearchForMate instance = new SearchForMate();

        public static SearchForMate Instance
        {
            get { return instance; }
        }

        static SearchForMate()
        {
        }

        //Hide constructor
        private SearchForMate()
        {
        }


        public override void Enter(Animal a)
        {
            Debug.Log("Time to find me a mate");
            a.nav.enabled = true;
            

            //we only change the state animation here for this purpose.
            currentStateAnimation = Running;
        }

        public override void Execute(Animal a)
        {
            //Searching behavior goes here

        }

        public override void Exit(Animal a)
        {
            Debug.Log("Wonder if i found a mate? Alright imma stop looking at least.");
        }
    }
}