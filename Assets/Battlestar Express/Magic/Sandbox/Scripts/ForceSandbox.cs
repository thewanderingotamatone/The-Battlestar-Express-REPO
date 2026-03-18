using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Magic;

public class ForceSandbox : MonoBehaviour
{

    // create a reference to the module
    public Magic.Modules.ForceModule force;

    // variables for position mapping
    public int strength;
    public float startPosition = 0;
    public float endPosition = 0.4f;

    void Start()
    {

    }

    void Update()
    {
        // access the property of the module
        strength = force.strength;

        // map data and set position
        float mappedPosition = ((float)strength / 100f) * (endPosition - startPosition) + startPosition;
        transform.localPosition = new Vector3(0.05f, mappedPosition, 0.2f);
    }
}
