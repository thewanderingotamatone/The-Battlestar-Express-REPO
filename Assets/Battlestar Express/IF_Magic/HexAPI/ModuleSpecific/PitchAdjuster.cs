using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Magic.Modules;

public class AudioAdjuster : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public string cutoffParameter = "cutoffAmt";

    [Header("Pitch Settings")]
    public float minPitch = 0.5f;
    public float maxPitch = 2.0f;

    [Header("Cutoff Settings")]
    public float minCutoff = 500f;
    public float maxCutoff = 5000f;

    [Header("Flex Module")]
    public FlexModule flexModule;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (flexModule == null)
        {
            Debug.Log("FlexModule is not assigned.");
        }

        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer is not assigned.");
        }
    }

    void Update()
    {
        if (flexModule != null)
        {
            // Normalize bend value to a range between 0 and 1
            float normalizedBend = Mathf.InverseLerp(0, 100, flexModule.bend);

            // Adjust pitch based on normalized bend value
            float pitch = Mathf.Lerp(minPitch, maxPitch, normalizedBend);

            // Set audio source pitch
            if (audioSource != null)
            {
                audioSource.pitch = pitch;
            }

            // Adjust cutoff parameter based on normalized bend value
            float cutoff = Mathf.Lerp(minCutoff, maxCutoff, normalizedBend);

            // Set audio mixer cutoff parameter
            if (audioMixer != null)
            {
                audioMixer.SetFloat(cutoffParameter, cutoff);
            }
        }
    }
}
