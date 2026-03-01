using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Magic.Modules;

public class ForceController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public int blendShapeIndex = 0; // Index of the blendshape to control
    public float maxForce = 100f; // Maximum force to be applied
    public float maxStrength = 100f; // Maximum strength value below 100
    public float speed = 50f; // Speed of increasing or decreasing the blendshape weight
    public bool simulate = false; // Toggle for using space bar simulation

    public List<Renderer> renderers; // List of renderers to modify
    public string shaderProperty = "_Panner"; // Shader property to lerp
    public int loops = 5; // Number of times to loop the 0-1 range
    public PlayableDirector playableDirector; // PlayableDirector to control the timeline

    public UnityEvent onReachZero; // Event triggered when blendshape weight reaches 0
    public UnityEvent onReachHundred; // Event triggered when blendshape weight reaches 100

    private float blendShapeWeight = 0f; // Current weight of the blendshape
    private ForceModule forceModule;
    private bool hasReachedZero = false;
    private bool hasReachedHundred = false;

    private PitchController pitchController;

    void Start()
    {
        // Try to find the ForceModule in the scene, even if it's disabled
        forceModule = FindObjectOfType<ForceModule>(true);

        // If ForceModule is not found or is disabled, enable simulation
        if (forceModule == null || !forceModule.isActiveAndEnabled)
        {
            simulate = true;
        }

        // Find the PitchController in the scene
        pitchController = FindObjectOfType<PitchController>();
    }

    void Update()
    {
        if (simulate)
        {
            // Simulation using space bar
            if (Input.GetKey(KeyCode.F))
            {
                // Increase the blendshape weight towards the maxForce
                blendShapeWeight = Mathf.MoveTowards(blendShapeWeight, maxForce, speed * Time.deltaTime);
            }
            else
            {
                // Decrease the blendshape weight towards 0
                blendShapeWeight = Mathf.MoveTowards(blendShapeWeight, 0, speed * Time.deltaTime);
            }
        }
        else
        {
            // Remap the strength value from 0-maxStrength to 0-100
            float normalizedStrength = Mathf.Clamp(forceModule.strength, 0, maxStrength) / maxStrength * 100f;
            blendShapeWeight = normalizedStrength;
        }

        // Apply the blendshape weight
        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, blendShapeWeight);

        // Calculate the shader property value
        float shaderValue = (blendShapeWeight / 100f) * loops % 1f;
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                material.SetFloat(shaderProperty, shaderValue);
            }
        }

        // Control the timeline
        if (playableDirector != null)
        {
            float timelineProgress = blendShapeWeight / 100f;
            playableDirector.time = timelineProgress * playableDirector.duration;
            playableDirector.Evaluate();
        }

        // Trigger UnityEvents
        if (blendShapeWeight == 0 && !hasReachedZero)
        {
            hasReachedZero = true;
            hasReachedHundred = false;
            onReachZero.Invoke();
        }
        else if (blendShapeWeight == 100 && !hasReachedHundred)
        {
            hasReachedHundred = true;
            hasReachedZero = false;
            onReachHundred.Invoke();
        }
        else if (blendShapeWeight > 0 && blendShapeWeight < 100)
        {
            hasReachedZero = false;
            hasReachedHundred = false;
        }

        // Play the pitch sequence based on force strength
        if (pitchController != null)
        {
            pitchController.PlayNoteBasedOnStrength(blendShapeWeight);
        }
    }
}
