using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Magic.Modules;

public class ColorSync : MonoBehaviour
{
    public GameObject targetObject;  // The GameObject whose material color we want to set

    private ColorModule colorModule;
    private Renderer targetRenderer;
    public AudioSource SFXSource;
    public AudioClip colorClip;

    public float minPitch;
    public float maxPitch;
    public float minRed;
    public float maxRed;

    void Start()
    {
        // Find the ColorModule component in the scene
        colorModule = FindObjectOfType<ColorModule>();

        // If no ColorModule component is found, log a warning
        if (colorModule == null)
        {
            Debug.LogWarning("No ColorModule component found in the scene.");
            return;
        }

        // Get the Renderer component of the target object
        if (targetObject != null)
        {
            targetRenderer = targetObject.GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                Debug.LogWarning("No Renderer component found on the target GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("No target GameObject assigned.");
        }
    }

    void Update()
    {
        // If the ColorModule and the Renderer are found, update the target object's color
        if (colorModule != null && targetRenderer != null)
        {
            // Set the base color of the target object's material to the color from the ColorModule
            targetRenderer.material.color = colorModule.color;

            SFXSource.pitch = Mathf.Lerp(minPitch, maxPitch, (colorModule.color.r - minRed) / (maxRed - minRed));
            if (!SFXSource.isPlaying)
            {
                SFXSource.Play();
            }
        }
    }
}
