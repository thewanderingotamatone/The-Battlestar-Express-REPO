using System.Collections;
using UnityEngine;
using UnityEditor;

public class PitchShifter : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    private float baseFrequency = 440f; // A4 frequency
    private float totalDuration = 2f; // Total duration in seconds
    private float noteDuration; // Duration of each note
    public float delayTime = 1f;
    public int numberOfNotes = 8; // Number of notes to drop down
    public bool ascending = true; // Determines the direction of the scale

    private float[] minorScaleIntervals;

    // Method to play the scale
    public void Play()
    {
    //     // Validate the number of notes
    //     if (numberOfNotes < 8) numberOfNotes = 8;
    //     if (numberOfNotes > 16) numberOfNotes = 16;

        // Generate minor scale intervals
        minorScaleIntervals = new float[numberOfNotes];
        float intervalFactor = Mathf.Pow(0.5f, 1f / (numberOfNotes - 1));
        for (int i = 0; i < numberOfNotes; i++)
        {
            minorScaleIntervals[i] = ascending ? Mathf.Pow(intervalFactor, i) : Mathf.Pow(intervalFactor, numberOfNotes - 1 - i);
        }

        noteDuration = (totalDuration - (0.01f * numberOfNotes)) / numberOfNotes;
        StartCoroutine(PlayScale());
    }

    public void ReverseDirectionPlay()
    {
        ascending = !ascending;
        Play();
        ascending = !ascending;
    }

    private IEnumerator PlayScale()
    {
        for (int i = 0; i < minorScaleIntervals.Length; i++)
        {
            float pitch = minorScaleIntervals[i];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);

            yield return new WaitForSeconds(noteDuration + (0.01f * delayTime)); // Add 10 milliseconds gap
        }
    }
}

// Custom Editor to add a button in the Inspector
/*[CustomEditor(typeof(PitchShifter))]
public class PitchShifterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PitchShifter pitchShifter = (PitchShifter)target;
        if (GUILayout.Button("Play Scale"))
        {
            pitchShifter.Play();
        }
    }
}*/
