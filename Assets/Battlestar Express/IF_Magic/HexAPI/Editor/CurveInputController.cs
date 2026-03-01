using UnityEngine;

public class CurveInputController : MonoBehaviour
{
    [Header("Input Curve Settings")]
    public AnimationCurve lookCurve; // AnimationCurve input for horizontal rotation
    public float deadZone = 0.1f; // Deadzone threshold for the input

    [Header("Output Value")]
    public float curveOutput; // Public field to store the curve output

    private void Update()
    {
        // Get horizontal input (assuming the input axis is named "Horizontal")
        float horizontalInput = Input.GetAxis("Horizontal");

        // Apply deadzone
        if (Mathf.Abs(horizontalInput) < deadZone)
        {
            horizontalInput = 0f;
        }

        // Clamp the input between -1 and 1
        horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);

        // Get the corresponding Y value from the animation curve
        curveOutput = lookCurve.Evaluate(horizontalInput);
    }

    // Method to get the curve output value
    public float GetCurveOutput()
    {
        return curveOutput;
    }
}
