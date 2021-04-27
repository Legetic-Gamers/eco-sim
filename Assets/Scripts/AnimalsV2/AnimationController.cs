/*
 * Author: Alexander L.V
 */

using System;
using System.Linq;
using UnityEngine;

namespace AnimalsV2
{
    public class AnimationController : MonoBehaviour
    {
        private AnimalController animal;
        private Animator animator;
        private float defaultSpeed;
        
        [HideInInspector]
        private Camera camera;

        private float transitionSpeed = 0.1f;

        private const float cameraDistanceThreshold = 65f;  //threshold in which lower distance to camera will render animation

        public void Init()
        {
            animal = GetComponent<AnimalController>();
            animator = GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
            defaultSpeed = animator.speed;
            camera = Camera.main;
            EventSubscribe();
        }

        public void OnDestroy()
        {
            EventUnsubscribe();
        }

        //only animate when close to camera and when timescale is below a threshold
        private void Update()
        {
            if (Time.timeScale < 5)
            {
                if (animator.enabled && !IsCloseToCamera())
                {
                    animator.enabled = false;
                    //Debug.Log("disable animator");
                } else if (!animator.enabled && IsCloseToCamera())
                {
                    animator.enabled = true;
                    animator.Rebind();
                    //Debug.Log("enable animator");
                    FSM_OnStateEnter(animal.fsm.currentState);
                    
                }    
            }
            else
            {
                if (animator.enabled)
                {
                    animator.enabled = false;
                }
            }
        }

        public AnimationController()
        {
            //Get access to animal to animate
            //this.animal = animal;
            
            //Get access to Animator to animate the animal.
            //animator = this.animal.GetComponent<Animator>();
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
        }

        private void PlayAnimation(State state)
        {
            animator.Play("Base Layer." + state.GetStateAnimation(),0 ,0 );
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
        
        
        bool IsCloseToCamera()
        {
            return Vector3.Distance(camera.transform.position, animal.transform.position) < cameraDistanceThreshold;
        }
        
    }
}