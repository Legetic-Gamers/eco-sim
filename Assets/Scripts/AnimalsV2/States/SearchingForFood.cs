using UnityEngine;
using static FSM.StateAnimation;

namespace FSM
{
    public sealed class SearchingForFood : FSMState<Animal>
    {
        private static readonly SearchingForFood instance = new SearchingForFood();

        public static SearchingForFood Instance => instance;

        static SearchingForFood() {}
        private SearchingForFood() {}

        public override void Enter(Animal entity)
        {
            currentStateAnimation = Running;
        }

        public override void Execute(Animal entity)
        {
            
        }

        public override void Exit(Animal entity)
        {
            throw new System.NotImplementedException();
        }
    }
}