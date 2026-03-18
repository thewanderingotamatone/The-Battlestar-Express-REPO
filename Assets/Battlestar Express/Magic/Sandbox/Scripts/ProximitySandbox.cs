using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Magic;

public class ProximitySandbox : MonoBehaviour
{

    // create a reference to the module
    public Magic.Modules.ProximityModule proximity;

    // variables for particle burst
    public int prox;
    public ParticleSystem particles;
    private bool watch = false;

    void Start()
    {

    }


    void Update()
    {
        // access the property of the module
        prox = proximity.raw;

        var emission = particles.emission;

        // activate particles in range
        if (prox > 230)
        {
            if (watch != true)
            {
                watch = true;
                emission.enabled = true;
                particles.Play();
            }
        }
        else
        {
            emission.enabled = false;
            watch = false;
        }

    }
}
