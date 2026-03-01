using UnityEditor;
using UnityEngine;
using Magic;
using System.Collections.Generic;

[CustomEditor(typeof(ModuleManager))]
public class ModuleManagerEditor : Editor
{
    private Texture2D backgroundImage;
    private ModuleDataVisualizer dataVisualizer;
    private List<Color> progressBarColors;

    private void OnEnable()
    {
        // Load the background image from the Resources folder
        backgroundImage = (Texture2D)Resources.Load("Logo");

        if (backgroundImage == null)
        {
            Debug.LogError("Background image not found. Make sure the image is placed in Assets/Resources and named 'Logo.png'.");
        }

        // Initialize the data visualizer
        dataVisualizer = FindObjectOfType<ModuleDataVisualizer>();
        if (dataVisualizer == null)
        {
            Debug.LogError("ModuleDataVisualizer not found in the scene.");
        }

        // Initialize progress bar colors with adjustments
        Color startColor = new Color(1.0f, 0.0f, 1.0f); // Magenta (Pinkish Purple)
        Color endColor = new Color32(1, 239, 172, 255); // Light Green
        progressBarColors = EditorUtilities.InitializeProgressBarColors(startColor, endColor, 8);
    }

    public override void OnInspectorGUI()
    {
        ModuleManager moduleManager = (ModuleManager)target;

        // Draw the dynamic ellipse
        DrawDynamicEllipse();

        if (backgroundImage != null)
        {
            // Render the background image preserving its original dimensions
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(backgroundImage);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginVertical(GUI.skin.box);

        // Header with the new checkbox
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Device Ports", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Raw Values", GUILayout.Width(70));
        moduleManager.showRawValues = EditorGUILayout.Toggle(moduleManager.showRawValues, GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < moduleManager.portConfigurations.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{i + 1}", GUILayout.Width(20));

            // Set the width of the enum popup to match the widest item
            Color backgroundColor = GUI.backgroundColor;
            if (moduleManager.portConfigurations[i] != ModuleType.None)
            {
                backgroundColor = progressBarColors[i];
            }

            GUIStyle enumStyle = new GUIStyle(EditorStyles.popup);
            if (moduleManager.portConfigurations[i] != ModuleType.None)
            {
                enumStyle.normal.background = EditorUtilities.MakeTex(2, 2, backgroundColor);
            }

            moduleManager.portConfigurations[i] = (ModuleType)EditorGUILayout.EnumPopup(moduleManager.portConfigurations[i], enumStyle, GUILayout.Width(100));

            // Add visualization
            if (dataVisualizer != null)
            {
                float data = dataVisualizer.GetPortData(i + 1);
                float min = dataVisualizer.GetPortMinValue(i + 1);
                float max = dataVisualizer.GetPortMaxValue(i + 1);

                if (moduleManager.showRawValues)
                {
                    EditorUtilities.DrawProgressBar(data, min, max, $"{i + 1}", true, progressBarColors[i]);
                }
                else
                {
                    float normalizedValue = (data - min) / (max - min) * 100f;
                    EditorUtilities.DrawProgressBar(normalizedValue, 0, 100, $"{i + 1}", false, progressBarColors[i]);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Update Modules"))
        {
            moduleManager.UpdateModules();
        }

        EditorGUILayout.EndVertical();

        EditorUtility.SetDirty(target);
    }

    private void DrawDynamicEllipse()
    {
        Rect gameViewRect = EditorGUIUtility.GetMainWindowPosition();
        Vector2 mousePosition = Event.current.mousePosition;

        float xRatio = Mathf.Clamp01(mousePosition.x / gameViewRect.width);
        float yRatio = Mathf.Clamp01(mousePosition.y / gameViewRect.height);

        float width = Mathf.Lerp(128, 10, Mathf.Abs(0.5f - xRatio) * 2);
        float height = Mathf.Lerp(128, 10, Mathf.Abs(0.5f - yRatio) * 2);

        Rect ellipseRect = new Rect((EditorGUIUtility.currentViewWidth - 128) / 2 + 6.4f, 12 - 6.4f, 128, 128);

        Handles.BeginGUI();
        for (int i = 0; i < progressBarColors.Count; i++)
        {
            float t = (float)i / (progressBarColors.Count - 1);
            float adjustedWidth = Mathf.Lerp(128, 10, Mathf.Abs(0.5f - xRatio) * 2 * t);
            float adjustedHeight = Mathf.Lerp(128, 10, Mathf.Abs(0.5f - yRatio) * 2 * t);

            Handles.color = progressBarColors[i];
            Handles.DrawWireDisc(ellipseRect.center, Vector3.forward, adjustedWidth / 2, 4);
            Handles.DrawWireDisc(ellipseRect.center, Vector3.forward, adjustedHeight / 2, 4);
        }
        Handles.EndGUI();
    }
}
