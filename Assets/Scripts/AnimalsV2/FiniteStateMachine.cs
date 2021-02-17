/*
 * Authors: Johan A, Alexander L.V.
 */

namespace AnimalsV2
 {
     /// <summary>
     /// State Machine handles States. 
     /// </summary>
     public class StateMachine
    {
        public State CurrentState { get; private set; }

        /// <summary>
        /// Start the state machine in a non-empty state
        /// </summary>
        /// <param name="startingState"> State to start in (Idle) </param>
        public void Initialize(State startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        /// <summary>
        /// Changing states. 
        /// </summary>
        /// <param name="newState"> State to change into. </param>
        public void ChangeState(State newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            newState.Enter();
        }
    }
}