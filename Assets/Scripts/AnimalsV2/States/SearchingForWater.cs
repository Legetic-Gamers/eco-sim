using UnityEngine;
using static FSM.StateAnimation;

namespace FSM
{
    public sealed class SearchingForWater : FSMState<Animal>
    {

        private static readonly SearchingForWater instance = new SearchingForWater();

        public static SearchingForWater Instance => instance;

        static SearchingForWater() {}

        private SearchingForWater() {}

        public override void Enter(Animal entity)
        {
            currentStateAnimation = Running;
        }

        public override void Execute(Animal entity)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit(Animal entity)
        {
            throw new System.NotImplementedException();
        }
    }
}