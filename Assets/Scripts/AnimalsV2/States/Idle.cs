using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;
using static FSM.StateAnimation;



//Author: Alexander LV
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
namespace FSM
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public sealed class Idle : FSMState<Animal>
    {
        static readonly Idle instance = new Idle();

        public static Idle Instance
        {
            get { return instance; }
        }

        static Idle()
        {
        }

        private Idle()
        {
        }


        public override void Enter(Animal a)
        {
            //Debug.Log("Idleing...");

            currentStateAnimation = StateAnimation.Idle;
        }

        public override void Execute(Animal a)
        {
        }

        public override void Exit(Animal a)
        {
        }
    }
}