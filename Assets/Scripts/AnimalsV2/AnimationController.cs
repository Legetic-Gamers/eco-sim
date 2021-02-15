
using UnityEngine;

namespace FSM
{
    

public class AnimationController : MonoBehaviour
{
    private Animal animal;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //Get access to animal to animate
        animal = GetComponent<Animal>();

        //Get access to Animator to animate the animal.
        animator = GetComponent<Animator>();

        //Listen to state changes of the animals states to update animations.
        animal.FSM.OnStateEnter += FSM_OnStateEnter;
        animal.FSM.OnStateExecute += FSM_OnOnStateExecute;
        animal.FSM.OnStateExit += FSM_OnOnStateExit;

    }

    //Animation parameters which need updating on state enter.
    private void FSM_OnStateEnter(FSMState<Animal> state)
    {
        Debug.Log("Enter " + state.ToString() +" Animation");
        
        animator?.CrossFade("Base Layer." + state,0.5f,0);
        
        
        
    }
    //Animation parameters which need updating every frame
    //Pretty much a very indirect Update() that goes through the FSM.
    //Has access to the state that is being executed.
    private void FSM_OnOnStateExecute(FSMState<Animal> state)
    {
        //Use update() instead most likely
        animator.SetFloat("runningSpeed",animal.Hunger);
    }

    //Animation parameters which need updating once a state ends
    private void FSM_OnOnStateExit(FSMState<Animal> state)
    {
        Debug.Log("Exit " + state.ToString() +" Animation");
        animator?.CrossFade("Base Layer." + state,0.5f,0);
        //animator.Play("Base Layer." + state,0);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
}