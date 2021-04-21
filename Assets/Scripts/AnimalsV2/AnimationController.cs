/*
 * Author: Alexander L.V
 */

using System.Linq;
using UnityEngine;

namespace AnimalsV2
{
    public class AnimationController
    {
        private AnimalController animal;
        private Animator animator;

        private float transitionSpeed = 0.1f;


        public AnimationController(AnimalController animal)
        {
            //Get access to animal to animate
            this.animal = animal;
            
            //Get access to Animator to animate the animal.
            animator = this.animal.GetComponent<Animator>();
            //animator.Play("Base Layer." + StateAnimation.Walking,0);
        }

        public void EventSubscribe()
        {
            //Listen to state changes of the animals states to update animations.
            animal.fsm.OnStateEnter += FSM_OnStateEnter;
            animal.fsm.OnStateLogicUpdate += FSM_OnStateLogicUpdate;
        }

        public void EventUnsubscribe()
        {
            animal.fsm.OnStateEnter -= FSM_OnStateEnter;
            animal.fsm.OnStateLogicUpdate -= FSM_OnStateLogicUpdate;
        }

        //Animation parameters which need updating on state enter.
        private void FSM_OnStateEnter(State state)
        {
            //animator.SetFloat("runningSpeed",animal.animalModel.GetSpeedPercentage);
            animator.CrossFade("Base Layer." + state.GetStateAnimation(), transitionSpeed, 0);
            
            Debug.Log("State:  " + state + "   " + "Animation: " + animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        }

        //Animation parameters which need updating every frame
        //Pretty much a very indirect Update() that goes through the FSM.
        //Has access to the state that is being executed.
        private void FSM_OnStateLogicUpdate(State state)
        {
            //Use update() instead most likely
            //animator.SetFloat("runningSpeed",animal.Hunger);
            
            //If animation has changed.
            // if (!animator.GetCurrentAnimatorStateInfo(0).IsName(state.GetStateAnimation()))
            // {
            //     animator?.CrossFade("Base Layer." + state.GetStateAnimation(), transitionSpeed, 0);
            // }
        }
        
    }
}