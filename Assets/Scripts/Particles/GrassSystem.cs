using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Stop the simulation right away.
        GetComponent<ParticleSystem>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
