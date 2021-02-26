/*
 * Author: Alexander L.V
 */

using UnityEngine;

namespace AnimalsV2
{
    public class AnimationController
    {
        private AnimalController animal;
        private Animator animator;

        private float transitionSpeed = 0.2f;


        public AnimationController(AnimalController animal)
        {
            //Get access to animal to animate
            this.animal = animal;

            //Get access to Animator to animate the animal.
            animator = this.animal.GetComponent<Animator>();

            //Listen to state changes of the animals states to update animations.
            animal.fsm.OnStateEnter += FSM_OnStateEnter;
            animal.fsm.OnStateLogicUpdate += FSM_OnStateLogicUpdate;
            animal.fsm.OnStateExit += FSM_OnStateExit;

            //Debug.Log("AnimationController listening to FSM");
        }

        public void EventUnsubscribe()
        {
            animal.fsm.OnStateEnter -= FSM_OnStateEnter;
            animal.fsm.OnStateLogicUpdate -= FSM_OnStateLogicUpdate;
            animal.fsm.OnStateExit -= FSM_OnStateExit;
        }

        //Animation parameters which need updating on state enter.
        private void FSM_OnStateEnter(State state)
        {
            //Debug.Log("Enter " + state.GetStateAnimation() +" Animation");

            animator?.CrossFade("Base Layer." + state.GetStateAnimation(), transitionSpeed, 0);
        }

        //Animation parameters which need updating every frame
        //Pretty much a very indirect Update() that goes through the FSM.
        //Has access to the state that is being executed.
        private void FSM_OnStateLogicUpdate(State state1)
        {
            //Use update() instead most likely
            //animator.SetFloat("runningSpeed",animal.Hunger);
        }

        //Animation parameters which need updating once a state ends
        private void FSM_OnStateExit(State state)
        {
            //Debug.Log("Exit " + state.GetStateAnimation() +" Animation");
            animator?.CrossFade("Base Layer." + state.GetStateAnimation(), transitionSpeed, 0);
            //animator.Play("Base Layer." + state,0);
        }

        public void PlayAnimation(StateAnimation animation, float transitionSpeed)
        {
            animator.CrossFade("Base Layer." + animation, transitionSpeed, 0);
        }
    }
}