using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Magic;

public class SpinSandbox : MonoBehaviour
{
    // create a reference to the module
    public Magic.Modules.SpinModule spin;

    // variable for rotation mapping
    public float rotation;

    void Start()
    {

    }


    void Update()
    {
        rotation = Mathf.Abs(((float)spin.rotation / 48) % 1);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.RoundToInt(rotation * 360),transform.eulerAngles.z);
    }
}
