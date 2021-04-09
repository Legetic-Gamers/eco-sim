using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    private ParticleSystem self;

    private void Start()
    {
        self = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Hit particle system on each animal does not have enough time to play if used like the other systems.
    /// Instead, detach the particle system from parent, and then play.
    /// That leaves the particle system remaining in the scene, however, so destroy it shortly afterwards.
    /// </summary>
    public void SelfDestructAfterPlayAndDetach()
    {
        self.transform.parent = null;
        self.Play();
        Destroy(gameObject, 1);
    }
}
