using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(MonoBehaviour), true)]
public class EditorButtonHandler : Editor
{
    private MethodInfo[] methods;
    private int selectedMethodIndex = 0;
    private List<string> methodNames = new List<string>();

    private void OnEnable()
    {
        // Get all public and non-public methods from the target script
        methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

        // Populate method names
        methodNames.Clear();
        foreach (var method in methods)
        {
            methodNames.Add(method.Name);
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (EditorApplication.isPlaying)
        {
            if (methods.Length > 0)
            {
                selectedMethodIndex = EditorGUILayout.Popup("Select Method", selectedMethodIndex, methodNames.ToArray());

                if (GUILayout.Button("Invoke Method"))
                {
                    MethodInfo method = methods[selectedMethodIndex];
                    method.Invoke(target, null);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No methods available to invoke.");
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Enter Play Mode to invoke methods.", MessageType.Info);
        }
    }
}
