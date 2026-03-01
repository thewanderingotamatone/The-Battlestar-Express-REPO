/*using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameObject))]
public class CustomGameObjectEditor : Editor
{
    private bool showActiveComponents = true;
    private bool showInactiveComponents = true;

    public override void OnInspectorGUI()
    {
        GameObject gameObject = (GameObject)target;

        // Get all components on the GameObject
        Component[] components = gameObject.GetComponents<Component>();
        List<Component> activeComponents = new List<Component>();
        List<Component> inactiveComponents = new List<Component>();

        // Separate components into active and inactive lists
        foreach (Component component in components)
        {
            if (component is Behaviour behaviour)
            {
                if (behaviour.enabled)
                {
                    activeComponents.Add(component);
                }
                else
                {
                    inactiveComponents.Add(component);
                }
            }
            else
            {
                activeComponents.Add(component);
            }
        }

        // Display active components
        showActiveComponents = EditorGUILayout.Foldout(showActiveComponents, "Active Components");
        if (showActiveComponents)
        {
            foreach (Component component in activeComponents)
            {
                DrawComponentInspector(component);
            }
        }

        // Display inactive components
        showInactiveComponents = EditorGUILayout.Foldout(showInactiveComponents, "Inactive Components");
        if (showInactiveComponents)
        {
            foreach (Component component in inactiveComponents)
            {
                DrawComponentInspector(component);
            }
        }

        // Draw the default inspector for other properties
        DrawDefaultInspector();
    }

    private void DrawComponentInspector(Component component)
    {
        if (component == null) return;

        SerializedObject serializedObject = new SerializedObject(component);
        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.BeginChangeCheck();

        while (property.NextVisible(false))
        {
            EditorGUILayout.PropertyField(property, true);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.EndVertical();
    }
}
*/