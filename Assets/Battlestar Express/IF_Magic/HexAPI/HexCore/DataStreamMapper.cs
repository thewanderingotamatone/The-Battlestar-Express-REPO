using UnityEngine;
using System.Collections.Generic;
using Magic;

public class DataStreamMapper : MonoBehaviour
{
    private ModuleManager moduleManager;

    [System.Serializable]
    public class VariableMapping
    {
        public bool overrideAutoSet = false;
        public int portNumber;
        public ModuleType selectedModule;
        public Component targetComponent;
        public string targetVariableName;
        public Vector2 range = new Vector2(0, 100);
        public int smoothingFrames;
        public bool modifyData;
    }

    public List<VariableMapping> variableMappings = new List<VariableMapping>();

    private void Start()
    {
        moduleManager = FindObjectOfType<ModuleManager>();
        if (moduleManager == null)
        {
            Debug.LogError("ModuleManager not found in the scene.");
        }
    }

    private void Update()
    {
        if (moduleManager == null) return;

        foreach (var mapping in variableMappings)
        {
            if (mapping.targetComponent == null || string.IsNullOrEmpty(mapping.targetVariableName)) continue;

            float dataValue = GetDataValue(mapping);
            if (mapping.modifyData)
            {
                dataValue = ReRangeData(dataValue, 0f, 100f, mapping.range.x, mapping.range.y);
                dataValue = SmoothData(dataValue, mapping.smoothingFrames);
            }

            SetTargetVariable(mapping.targetComponent, mapping.targetVariableName, dataValue);
        }
    }

    private float GetDataValue(VariableMapping mapping)
    {
        if (!mapping.overrideAutoSet)
        {
            if (mapping.portNumber > 0)
            {
                mapping.selectedModule = moduleManager.portConfigurations[mapping.portNumber - 1];
            }
            else if (mapping.selectedModule != ModuleType.None)
            {
                mapping.portNumber = GetPortForModule(mapping.selectedModule);
            }
        }

        return moduleManager.dataVisualizer.GetPortData(mapping.portNumber);
    }

    public int GetPortForModule(ModuleType moduleType)
    {
        for (int i = 0; i < moduleManager.portConfigurations.Count; i++)
        {
            if (moduleManager.portConfigurations[i] == moduleType)
            {
                return i + 1;
            }
        }
        return -1;
    }

    private float ReRangeData(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return ((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
    }

    private float SmoothData(float value, int smoothingFrames)
    {
        if (smoothingFrames <= 1) return value;

        Queue<float> smoothingQueue = new Queue<float>();
        float smoothingSum = 0f;

        if (smoothingQueue.Count >= smoothingFrames)
        {
            smoothingSum -= smoothingQueue.Dequeue();
        }

        smoothingQueue.Enqueue(value);
        smoothingSum += value;

        return smoothingSum / smoothingQueue.Count;
    }

    private void SetTargetVariable(Component targetComponent, string targetVariableName, float value)
    {
        var targetField = targetComponent.GetType().GetField(targetVariableName);
        if (targetField != null)
        {
            if (targetField.FieldType == typeof(float))
            {
                targetField.SetValue(targetComponent, value);
            }
        }
    }
}
