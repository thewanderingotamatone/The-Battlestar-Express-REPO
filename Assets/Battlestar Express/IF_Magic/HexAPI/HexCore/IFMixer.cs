using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Magic;
using Magic.Modules;

[ExecuteAlways]
public class IFMixer : MonoBehaviour
{
    private Stream dataStream;

    [HideInInspector]
    public List<PortData> parsedPortsData = new List<PortData>();

    public string[] organizedRawData = new string[8];

    private string previousData = "";
    private float timeSinceLastChange = 0f;
    private float maxTimeWithoutChange = 2f; // Maximum time without data change before trying to bond again

    private IFMagicSettings settings;
    private const string settingsPath = "Assets/Settings/IFMagicSettings.asset";
    private bool debug = true;

    public event Action OnPortConfigurationChanged;

    void Start()
    {
        // Find the Stream object in the scene
        dataStream = FindObjectOfType<Stream>();

        if (dataStream == null)
        {
            Debug.LogError("Stream object not found in the scene.");
        }
        else
        {
            if (!dataStream.connected)
            {
                dataStream.BondForce();
            }
        }
        #if UNITY_EDITOR
        settings = AssetDatabase.LoadAssetAtPath<IFMagicSettings>(settingsPath);
        #endif
        if (settings != null)
        {
            debug = settings.onScreenDebug;
        }
    }

    void Update()
    {
        if (dataStream != null && dataStream.connected)
        {
            if (dataStream.data != previousData)
            {
                previousData = dataStream.data;
                timeSinceLastChange = 0f;
                ParseDataStream(dataStream.data);
                OnPortConfigurationChanged?.Invoke();
            }
            else
            {
                timeSinceLastChange += Time.deltaTime;
                if (timeSinceLastChange > maxTimeWithoutChange)
                {
                    dataStream.BondForce();
                    timeSinceLastChange = 0f;
                }
            }
        }
        else if (dataStream != null && !dataStream.connected)
        {
            dataStream.BondForce();
        }
    }

    void ParseDataStream(string data)
    {
        parsedPortsData.Clear();

        // Split the data stream by semicolon to get each port's data
        string[] portsData = data.Split(';');

        // Iterate over each port's data
        for (int portIndex = 0; portIndex < portsData.Length; portIndex++)
        {
            string portData = portsData[portIndex];
            // Split by colon to get module and values
            string[] parts = portData.Split(':');
            int module = 0;
            string rawData = portData; // default raw data

            if (portIndex < 8 && parts.Length > 1)
            {
                // Parse module for ports 1-8
                int.TryParse(parts[0], out module);
                // Collect raw data
                rawData = string.Join(":", parts, 1, parts.Length - 1);
                organizedRawData[portIndex] = rawData;
            }
            else if (portIndex >= 8)
            {
                // Just collect raw data for ports 9-10
                rawData = string.Join(":", parts);
            }

            // Add the parsed data to the list
            parsedPortsData.Add(new PortData
            {
                PortNumber = portIndex + 1,
                Module = (ModuleType)module,
                RawData = rawData
            });
        }
    }

    public void ModuleChange(int portNumber, int moduleCodeNumber)
    {
        string outputCode = "1," + portNumber + "," + moduleCodeNumber;
        dataStream = FindObjectOfType<Stream>();
        dataStream.Output(outputCode);
        if (debug) Debug.Log("IFMIXER: " + dataStream + "  : " + outputCode);
    }

    [ContextMenu("Update Modules")]
    public void UpdateModules()
    {
        for (int i = 0; i < parsedPortsData.Count; i++)
        {
            if (i < 8) // Only update modules for ports 1-8
            {
                ModuleType moduleType = parsedPortsData[i].Module;
                if (moduleType != ModuleType.None)
                {
                    if (debug) Debug.Log(moduleType);
                    int moduleCodeNumber = (int)moduleType;
                    ModuleChange(i + 1, moduleCodeNumber);
                }
            }
        }
        for (int i = 0; i < parsedPortsData.Count; i++) // none overwrites
        {
            if (i < 8) // Only update modules for ports 1-8
            {
                ModuleType moduleType = parsedPortsData[i].Module;
                if (moduleType == ModuleType.None)
                {
                    if (debug) Debug.Log(moduleType);
                    int moduleCodeNumber = (int)moduleType;
                    ModuleChange(i + 1, moduleCodeNumber);
                }
            }
        }

        // Update the settings configuration to mirror the current port configurations
        if (settings != null)
        {
            settings.deviceConfiguration.portConfiguration.portModules = parsedPortsData.ConvertAll(pd => pd.Module).ToArray();
            #if UNITY_EDITOR
            EditorUtility.SetDirty(settings); // Mark the settings object as dirty to ensure changes are saved
            AssetDatabase.SaveAssets(); // Save the changes to the asset file
            #endif
        }

        OnPortConfigurationChanged?.Invoke();
    }

    [System.Serializable]
    public class PortData
    {
        public int PortNumber;
        public ModuleType Module;
        public string RawData;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(IFMixer))]
    public class IFMixerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            IFMixer parser = (IFMixer)target;

            DrawDefaultInspector();

            GUILayout.Label("Port Data Table:");
            for (int i = 0; i < parser.parsedPortsData.Count; i++)
            {
                var portData = parser.parsedPortsData[i];
                GUILayout.BeginHorizontal();

                if (i < 8) // Enum dropdown for ports 1-8
                {
                    GUILayout.Label((i + 1).ToString(), GUILayout.Width(30));

                    if (portData.Module == ModuleType.None)
                    {
                        GUILayout.Label("", GUILayout.Width(80));
                        var selectedModule = (ModuleType)EditorGUILayout.EnumPopup("", portData.Module, GUILayout.Width(20));
                        if (selectedModule != portData.Module)
                        {
                            parser.ModuleChange(i + 1, (int)selectedModule);
                            portData.Module = selectedModule;
                        }
                    }
                    else
                    {
                        var selectedModule = (ModuleType)EditorGUILayout.EnumPopup(portData.Module, GUILayout.Width(130));
                        if (selectedModule != portData.Module)
                        {
                            parser.ModuleChange(i + 1, (int)selectedModule);
                            portData.Module = selectedModule;
                        }
                    }

                    // Special Visualization for Joystick
                    if (portData.Module == ModuleType.Joystick)
                    {
                        string[] joystickData = portData.RawData.Split(',');
                        if (joystickData.Length == 2)
                        {
                            float xValue, yValue;
                            float.TryParse(joystickData[0], out xValue);
                            float.TryParse(joystickData[1], out yValue);
                            float minValue = 0f;
                            float maxValue = 4095f; // Replace with actual max value for the joystick

                            GUILayout.BeginVertical();
                            GUILayout.BeginHorizontal();
                            GUILayout.Label($"X: {joystickData[0]}", GUILayout.Width(50));
                            Rect xRect = EditorGUILayout.GetControlRect(false, 20);
                            EditorGUI.ProgressBar(xRect, xValue / maxValue, "");
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            GUILayout.Label($"Y: {joystickData[1]}", GUILayout.Width(50));
                            Rect yRect = EditorGUILayout.GetControlRect(false, 20);
                            EditorGUI.ProgressBar(yRect, yValue / maxValue, "");
                            GUILayout.EndHorizontal();
                            GUILayout.EndVertical();
                            i++; // Skip the next port since joystick takes two ports
                        }
                    }
                    // Visualization for Slider
                    else if (portData.Module == ModuleType.Slider)
                    {
                        int intValue;
                        if (int.TryParse(portData.RawData, out intValue))
                        {
                            float minValue = 0;
                            float maxValue = 4095; // Replace with the actual max value from the module

                            float progress = Mathf.InverseLerp(minValue, maxValue, intValue);
                            Rect rect = EditorGUILayout.GetControlRect(false, 20);
                            EditorGUI.Slider(rect, maxValue - intValue, minValue, maxValue);
                        }
                        else
                        {
                            GUILayout.Label(portData.RawData, GUILayout.Width(250));
                        }
                    }
                    // Visualization for Color
                    else if (portData.Module == ModuleType.Color)
                    {
                        string[] colorData = portData.RawData.Split(',');
                        if (colorData.Length == 4)
                        {
                            float a = float.Parse(colorData[0]) / 255f;
                            float r = float.Parse(colorData[1]) / 255f;
                            float g = float.Parse(colorData[2]) / 255f;
                            float b = float.Parse(colorData[3]) / 255f;

                            Color color = new Color(r, g, b, a);
                            Rect rect = EditorGUILayout.GetControlRect(false, 20);
                            EditorGUI.DrawRect(rect, color);
                        }
                        else
                        {
                            GUILayout.Label(portData.RawData, GUILayout.Width(250));
                        }
                    }
                    // General Visualization
                    else if (portData.Module == ModuleType.Glow)
                    {
                        // Output type visualization
                        DrawGlowModuleGUI(parser, portData);
                    }
                    // Visualization for Tone
                    else if (portData.Module == ModuleType.Tone)
                    {
                        DrawToneModuleGUI(parser, portData);
                    }
                    else if (portData.Module == ModuleType.Button)
                    {
                        // Bool type visualization
                        bool boolValue = portData.RawData == "1";
                        GUILayout.Label(boolValue ? "●" : "○", GUILayout.Width(250));
                    }
                    else
                    {
                        int intValue;
                        if (int.TryParse(portData.RawData, out intValue))
                        {
                            float minValue = 0;
                            float maxValue = 4095; // Replace with the actual max value from the module

                            float progress = Mathf.InverseLerp(minValue, maxValue, intValue);
                            Rect rect = EditorGUILayout.GetControlRect(false, 20);
                            EditorGUI.ProgressBar(rect, progress, intValue.ToString());
                        }
                        else
                        {
                            GUILayout.Label(portData.RawData == "0" ? "" : portData.RawData, GUILayout.Width(250));
                        }
                    }
                }
                else if (i == 8) // Power level for port 9
                {
                    DrawPowerVisualization(portData);
                }
                else if (i == 9) // IMU Data for port 10
                {
                    DrawIMUVisualization(portData);
                }

                GUILayout.EndHorizontal();
            }

            // Force the Inspector to refresh to show live data updates
            if (Application.isPlaying || !Application.isPlaying)
            {
                Repaint();
            }
        }

        private void DrawPowerVisualization(PortData portData)
        {
            float powerValue;
            if (float.TryParse(portData.RawData, out powerValue))
            {
                powerValue = Mathf.Clamp(powerValue, 1400, 2400);
                float normalizedPowerValue = (powerValue - 1400) / (2400 - 1400);
                Color powerColor = Color.green;
                if (normalizedPowerValue <= 0.2f)
                {
                    powerColor = Color.red;
                }
                else if (normalizedPowerValue <= 0.5f)
                {
                    powerColor = Color.yellow;
                }

                Rect rect = EditorGUILayout.GetControlRect(false, 20);
                EditorGUI.ProgressBar(rect, normalizedPowerValue, $"Battery {normalizedPowerValue * 100:F1}%");
                // Draw the rectangle
                rect.width /= 4f;
                EditorGUI.DrawRect(rect, powerColor);
            }
            else
            {
                GUILayout.Label(portData.RawData, GUILayout.Width(250));
            }
        }

        private void DrawIMUVisualization(PortData portData)
        {
            string[] imuParts = portData.RawData.Split('<')[0].Split(':');
            if (imuParts.Length >= 7)
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label("IMU Data - Quaternion", GUILayout.Width(150));
                GUILayout.Label("X: " + imuParts[0], GUILayout.Width(80));
                GUILayout.Label("Y: " + imuParts[1], GUILayout.Width(80));
                GUILayout.Label("Z: " + imuParts[2], GUILayout.Width(80));
                GUILayout.Label("W: " + imuParts[3], GUILayout.Width(80));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("IMU Data - Acceleration", GUILayout.Width(150));
                GUILayout.Label("X: " + imuParts[4], GUILayout.Width(80));
                GUILayout.Label("Y: " + imuParts[5], GUILayout.Width(80));
                GUILayout.Label("Z: " + imuParts[6], GUILayout.Width(80));
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                // Visualize the rotation
                Quaternion rotation = new Quaternion(float.Parse(imuParts[0]), float.Parse(imuParts[1]), float.Parse(imuParts[2]), float.Parse(imuParts[3]));
                rotation.Normalize();
                Vector3 acceleration = new Vector3(float.Parse(imuParts[4]), float.Parse(imuParts[5]), float.Parse(imuParts[6]));

                // Draw the rotation and acceleration visualization
                Handles.ArrowHandleCap(0, Vector3.zero, rotation, 1f, EventType.Repaint);
                Handles.color = Color.red;
                Handles.DrawLine(Vector3.zero, acceleration);
            }
            else
            {
                GUILayout.Label("IMU Data", GUILayout.Width(100));
                GUILayout.Label(portData.RawData, GUILayout.Width(250));
            }
        }

        private void DrawGlowModuleGUI(IFMixer parser, PortData portData)
        {
            GlowModule glowModule = FindObjectOfType<GlowModule>();
            if (glowModule != null)
            {
                GUILayout.BeginVertical();
                for (int j = 0; j < glowModule.color.Length; j++)
                {
                    glowModule.color[j] = EditorGUILayout.ColorField($"Color {j + 1}", glowModule.color[j]);
                }

                if (GUILayout.Button("Send Colors to Glow"))
                {
                    glowModule.SetColor(glowModule.color);
                }
                GUILayout.EndVertical();
            }
        }

        private void DrawToneModuleGUI(IFMixer parser, PortData portData)
        {
            ToneModule toneModule = FindObjectOfType<ToneModule>();
            if (toneModule != null)
            {
                GUILayout.BeginVertical();
                toneModule.frequency = EditorGUILayout.IntSlider("Frequency", toneModule.frequency, 0, 3000);
                toneModule.time = EditorGUILayout.IntField("Time (ms)", toneModule.time);

                if (GUILayout.Button("Send Tone"))
                {
                    toneModule.SetTone(toneModule.frequency, toneModule.time);
                }
                GUILayout.EndVertical();
            }
        }

        private void CollapseExpandComponents(IFMixer parser)
        {
            foreach (var component in parser.GetComponents<Component>())
            {
                var serializedObject = new SerializedObject(component);
                var property = serializedObject.GetIterator();
                property.NextVisible(true);
                if (property.name == "m_Script")
                {
                    continue;
                }

                if (component is PortData)
                {
                    if (property.FindPropertyRelative("Module").enumValueIndex == 0)
                    {
                        property.isExpanded = false;
                    }
                    else
                    {
                        property.isExpanded = true;
                    }
                }
            }
        }
    }
#endif
}
