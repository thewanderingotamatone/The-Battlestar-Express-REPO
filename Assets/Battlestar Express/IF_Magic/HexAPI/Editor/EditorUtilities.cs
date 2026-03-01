using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class EditorUtilities
{
    public static void DrawProgressBar(float value, float min, float max, string label, bool isRaw, Color color)
    {
        float fillPercentage = (value - min) / (max - min);
        Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(18));
        EditorGUI.DrawRect(rect, Color.grey); // Background color
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width * fillPercentage, rect.height), color); // Fill color
        EditorGUI.ProgressBar(rect, fillPercentage, isRaw ? $"{label}: {value}" : $"{label}: {Mathf.RoundToInt(value)}%");
        GUILayout.Space(5);
    }

    public static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public static List<Color> InitializeProgressBarColors(Color startColor, Color endColor, int count)
    {
        List<Color> result = new List<Color>();
        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1);
            Color interpolatedColor = Color.Lerp(startColor, endColor, t);
            result.Add(AdjustColor(interpolatedColor, -0.25f, 0.1f));
        }
        return result;
    }

    private static Color AdjustColor(Color color, float lightnessAdjustment, float blueAdjustment)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        v = Mathf.Clamp01(v + lightnessAdjustment);
        Color newColor = Color.HSVToRGB(h, s, v);
        newColor.b = Mathf.Clamp01(newColor.b + blueAdjustment);
        return newColor;
    }
}
