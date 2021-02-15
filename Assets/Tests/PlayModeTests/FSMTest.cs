﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FSMTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void FSMTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }
        
        // Testing the basic functionality of the FSM
        [Test]
        public void FSMTestBasicFunctionalityPasses()
        {
            GameObject g = new GameObject();
            

        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FSMTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
