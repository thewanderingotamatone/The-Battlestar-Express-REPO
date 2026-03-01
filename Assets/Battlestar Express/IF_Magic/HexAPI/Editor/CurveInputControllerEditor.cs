using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CurveInputController))]
public class CurveInputControllerEditor : Editor
{
    private void OnSceneGUI()
    {
        CurveInputController controller = (CurveInputController)target;

        // Get the animation curve
        AnimationCurve curve = controller.lookCurve;

        // Draw the animation curve in the Scene view
        Handles.DrawAAPolyLine(3f, GetCurvePoints(curve));

        // Draw deadzone lines
        DrawDeadZoneLines(controller.deadZone);
    }

    private Vector3[] GetCurvePoints(AnimationCurve curve)
    {
        int resolution = 100;
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (resolution - 1f);
            float x = Mathf.Lerp(-1f, 1f, t);
            float y = curve.Evaluate(x);
            points[i] = new Vector3(x, y, 0);
        }

        return points;
    }

    private void DrawDeadZoneLines(float deadZone)
    {
        // Convert deadzone to world space coordinates
        float deadZoneLeft = Mathf.Lerp(-1f, 1f, (deadZone + 1) / 2f);
        float deadZoneRight = Mathf.Lerp(-1f, 1f, (1 - deadZone) / 2f);

        // Draw vertical lines at deadzone boundaries
        Handles.color = Color.red;
        Handles.DrawLine(new Vector3(deadZoneLeft, -1, 0), new Vector3(deadZoneLeft, 1, 0));
        Handles.DrawLine(new Vector3(deadZoneRight, -1, 0), new Vector3(deadZoneRight, 1, 0));
    }
}
