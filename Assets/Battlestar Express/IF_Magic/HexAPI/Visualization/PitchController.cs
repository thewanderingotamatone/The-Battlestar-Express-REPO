using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PitchController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    public float delayTime = 1f; // Maximum delay time between notes
    public int numberOfNotes = 8; // Number of notes to drop down
    public bool ascending = true; // Determines the direction of the scale

    private float[] minorScaleIntervals;
    private int lastNoteIndex = -1;
    private Coroutine playNoteCoroutine = null;

    void Start()
    {
        // Validate the number of notes
        if (numberOfNotes < 8) numberOfNotes = 8;
        if (numberOfNotes > 16) numberOfNotes = 16;

        // Generate minor scale intervals
        minorScaleIntervals = new float[numberOfNotes];
        float intervalFactor = Mathf.Pow(0.5f, 1f / (numberOfNotes - 1));
        for (int i = 0; i < numberOfNotes; i++)
        {
            minorScaleIntervals[i] = Mathf.Pow(intervalFactor, i);
        }

        if (!ascending)
        {
            System.Array.Reverse(minorScaleIntervals);
        }
    }

    // Method to play a note based on force strength
    public void PlayNoteBasedOnStrength(float forceStrength)
    {
        int noteIndex = Mathf.Clamp(Mathf.FloorToInt((forceStrength / 100f) * numberOfNotes), 0, numberOfNotes - 1);

        if (noteIndex != lastNoteIndex)
        {
            lastNoteIndex = noteIndex;

            if (playNoteCoroutine != null)
            {
                StopCoroutine(playNoteCoroutine);
            }

            playNoteCoroutine = StartCoroutine(PlayNoteCoroutine(noteIndex));
        }
    }

    private IEnumerator PlayNoteCoroutine(int noteIndex)
    {
        float pitch = minorScaleIntervals[noteIndex];
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClip);

        yield return new WaitForSeconds(delayTime);
    }
}

// Custom Editor to add a button in the Inspector
/*[CustomEditor(typeof(PitchController))]
public class PitchControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PitchController pitchController = (PitchController)target;
        if (GUILayout.Button("Play Scale"))
        {
            pitchController.PlayNoteBasedOnStrength(100); // Play scale with max force strength for testing
        }
    }
}*/
