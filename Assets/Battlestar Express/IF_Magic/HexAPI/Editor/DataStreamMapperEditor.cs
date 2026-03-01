using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Magic;

[CustomEditor(typeof(DataStreamMapper))]
public class DataStreamMapperEditor : Editor
{
    private SerializedProperty variableMappingsProp;
    private DataStreamMapper dataStreamMapper;
    private ModuleManager moduleManager;

    private void OnEnable()
    {
        variableMappingsProp = serializedObject.FindProperty("variableMappings");
        dataStreamMapper = (DataStreamMapper)target;
        moduleManager = FindObjectOfType<ModuleManager>();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Variable Mappings", EditorStyles.boldLabel);

        if (variableMappingsProp == null || variableMappingsProp.arraySize == 0)
        {
            EditorGUILayout.LabelField("No data is currently mapped.");
        }

        for (int i = 0; i < variableMappingsProp.arraySize; i++)
        {
            var mappingProp = variableMappingsProp.GetArrayElementAtIndex(i);
            if (mappingProp == null) continue;

            var overrideAutoSetProp = mappingProp.FindPropertyRelative("overrideAutoSet");
            var portNumberProp = mappingProp.FindPropertyRelative("portNumber");
            var selectedModuleProp = mappingProp.FindPropertyRelative("selectedModule");
            var targetComponentProp = mappingProp.FindPropertyRelative("targetComponent");
            var targetVariableNameProp = mappingProp.FindPropertyRelative("targetVariableName");
            var rangeProp = mappingProp.FindPropertyRelative("range");
            var smoothingFramesProp = mappingProp.FindPropertyRelative("smoothingFrames");
            var modifyDataProp = mappingProp.FindPropertyRelative("modifyData");
            var dataFormatProp = mappingProp.FindPropertyRelative("dataFormat");

            string description = $"{portNumberProp.intValue} {selectedModuleProp.enumNames[selectedModuleProp.enumValueIndex]} → {ObjectNames.NicifyVariableName(targetVariableNameProp.stringValue)}";

            mappingProp.isExpanded = EditorGUILayout.Foldout(mappingProp.isExpanded, description, true);
            if (mappingProp.isExpanded)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Port", GUILayout.Width(40));
                EditorGUILayout.LabelField("Module", GUILayout.Width(60));
                EditorGUILayout.LabelField("Manual Port & Module", GUILayout.Width(140));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (portNumberProp != null)
                {
                    EditorGUILayout.IntSlider(portNumberProp, 1, 8, GUIContent.none, GUILayout.Width(100));
                }

                if (selectedModuleProp != null)
                {
                    GUI.enabled = !overrideAutoSetProp.boolValue;
                    EditorGUILayout.PropertyField(selectedModuleProp, GUIContent.none, GUILayout.Width(100));
                    GUI.enabled = true;
                }

                if (overrideAutoSetProp != null)
                {
                    EditorGUILayout.PropertyField(overrideAutoSetProp, GUIContent.none, GUILayout.Width(20));
                }
                EditorGUILayout.EndHorizontal();

                if (overrideAutoSetProp != null && !overrideAutoSetProp.boolValue)
                {
                    if (portNumberProp != null && portNumberProp.intValue > 0)
                    {
                        selectedModuleProp.enumValueIndex = (int)moduleManager.portConfigurations[portNumberProp.intValue - 1];
                    }
                    else if (selectedModuleProp != null && selectedModuleProp.enumValueIndex > 0)
                    {
                        portNumberProp.intValue = dataStreamMapper.GetPortForModule((ModuleType)selectedModuleProp.enumValueIndex);
                    }
                }

                EditorGUILayout.LabelField("Target Component", EditorStyles.boldLabel);
                if (targetComponentProp != null)
                {
                    EditorGUILayout.PropertyField(targetComponentProp);
                    var targetComponent = targetComponentProp.objectReferenceValue as Component;
                    if (targetComponent != null)
                    {
                        var components = targetComponent.GetComponents<Component>();
                        var componentNames = new string[components.Length];
                        for (int j = 0; j < components.Length; j++)
                        {
                            componentNames[j] = components[j].GetType().Name;
                        }

                        int selectedComponentIndex = System.Array.IndexOf(components, targetComponent);
                        selectedComponentIndex = EditorGUILayout.Popup(selectedComponentIndex, componentNames);
                        targetComponent = components[selectedComponentIndex];
                        targetComponentProp.objectReferenceValue = targetComponent;

                        var targetVariables = GetPublicFloatVariables(targetComponent);
                        var targetVariableNames = new string[targetVariables.Count];
                        for (int j = 0; j < targetVariables.Count; j++)
                        {
                            targetVariableNames[j] = ObjectNames.NicifyVariableName(targetVariables[j].Name);
                        }

                        int selectedIndex = System.Array.IndexOf(targetVariableNames, targetVariableNameProp.stringValue);
                        int newIndex = EditorGUILayout.Popup("Target Variable", selectedIndex, targetVariableNames);
                        if (newIndex >= 0 && newIndex < targetVariableNames.Length)
                        {
                            targetVariableNameProp.stringValue = targetVariables[newIndex].Name;
                        }
                    }
                }

                EditorGUILayout.BeginHorizontal();
                string modifyButtonText = modifyDataProp != null && modifyDataProp.boolValue ? "Unmodify Data" : "Modify Data";
                if (GUILayout.Button(modifyButtonText, GUILayout.Width(100)))
                {
                    if (modifyDataProp != null)
                    {
                        modifyDataProp.boolValue = !modifyDataProp.boolValue;
                    }
                }

                if (GUILayout.Button("Duplicate", GUILayout.Width(100)))
                {
                    variableMappingsProp.InsertArrayElementAtIndex(i);
                }

                GUIStyle deleteButtonStyle = new GUIStyle(GUI.skin.button);
                deleteButtonStyle.normal.textColor = Color.white;
                deleteButtonStyle.normal.background = MakeTex(2, 2, new Color(0.788f, 0.035f, 0.165f)); // #C9092A
                deleteButtonStyle.fontStyle = FontStyle.Bold;

                if (GUILayout.Button("Delete", deleteButtonStyle, GUILayout.Width(100)))
                {
                    variableMappingsProp.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();

                if (modifyDataProp != null && modifyDataProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Data Modifications", EditorStyles.boldLabel);
                    
                    if (dataFormatProp != null)
                    {
                        EditorGUILayout.PropertyField(dataFormatProp, new GUIContent("Data Format"));
                    }
                    
                    if (rangeProp != null)
                    {
                        Vector2 range = rangeProp.vector2Value;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Clamp", GUILayout.Width(40));
                        EditorGUILayout.MinMaxSlider(ref range.x, ref range.y, 0, 100);
                        EditorGUILayout.LabelField($"{range.x}", GUILayout.Width(50));
                        EditorGUILayout.LabelField($"{range.y}", GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                        rangeProp.vector2Value = range;
                    }

                    if (smoothingFramesProp != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Data Smoothing", GUILayout.Width(100));
                        EditorGUILayout.IntSlider(smoothingFramesProp, 0, 20, GUIContent.none);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }

        if (GUILayout.Button("Add Mapping", GUILayout.Width(100)))
        {
            variableMappingsProp.InsertArrayElementAtIndex(variableMappingsProp.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private List<FieldInfo> GetPublicFloatVariables(Component component)
    {
        var fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
        var floatFields = new List<FieldInfo>();
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(float))
            {
                floatFields.Add(field);
            }
        }
        return floatFields;
    }

    private Texture2D MakeTex(int width, int height, Color col)
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
}
