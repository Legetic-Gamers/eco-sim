using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
            // Dummy test to see if tests works in unity
            float[,] noiseMap = Noise.GenerateNoiseMap(288, 293, 14, 36, 17, 0.334f, 2.92f, new Vector2(0,0));
            Assert.AreEqual(TextureGenerator.TextureFromHeightMap(noiseMap).height, 293);
            
        }

    }
}
