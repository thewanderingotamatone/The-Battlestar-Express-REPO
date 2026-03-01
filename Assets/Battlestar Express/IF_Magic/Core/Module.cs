using System;
using UnityEngine;
using UnityEditor;

namespace Magic
{
    [ExecuteAlways]
    [System.Serializable]
    public class Module : MonoBehaviour
    {
        private static IFMagicSettings settings;
        private const string settingsPath = "IFMagicSettings"; // Assuming the file is in Assets/Resources

        public static bool debug;

        protected int moduleNumber;

        [ConditionalHide("debug", true)]
        public bool simulated = false;

        [ConditionalHide("debug", true)]
        public bool ManuallySetSource = false;
        [ConditionalHide("ManuallySetSource")]
        public Hardware hardware;
        [ConditionalHide("ManuallySetSource")]
        public int portNumber;

        private int portIndex;
        private bool found = false;

        private string rawData;
        [HideInInspector]
        public string[] parsedData;
        [HideInInspector]
        public string[] processedData;

        [HideInInspector]
        public int dataLength;
        [HideInInspector]
        public string minValue;
        [HideInInspector]
        public string maxValue;
        [HideInInspector]
        public float svr = 0f;      
        [HideInInspector]
        public Vector2 minMax;

        public enum DataTypes { String, Integer, Float, Bool };
        [HideInInspector]
        public DataTypes dataType;

        public event Action<string> OnDataUpdated;

        private void OnEnable()
        {
            LoadSettings();
            ApplyDebugSetting();
        }

        private void LoadSettings()
        {
            settings = Resources.Load<IFMagicSettings>(settingsPath);
            if (settings == null)
            {
                Debug.LogWarning("Settings not found at " + settingsPath);
                return;
            }
        }

        private void ApplyDebugSetting()
        {
            if (settings != null)
            {
                debug = settings.onScreenDebug;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        private void OnValidate()
        {
            LoadSettings();
            ApplyDebugSetting();
        }

        public virtual void Start()
        {
            FindAssociatedPort();
        }

        public virtual void FindAssociatedPort()
        {
            parsedData = new string[dataLength];
            processedData = new string[dataLength];

            for (int i = 0; i < dataLength; i++)
            {
                parsedData[i] = "0";
                processedData[i] = "0";
            }

            if (!simulated)
            {
                if (!ManuallySetSource)
                {
                    AutoSetDataSource();
                }
                else
                {
                    portIndex = portNumber - 1;
                    found = true;
                }
            }

            if (portIndex == -1)
            {
                found = false;
            }
        }

        public virtual void Update()
        {
            FindAssociatedPort();

            if (!found)
            {
                Debug.Log(GetModule() + " module isn't plugged in or configured to a port!");
                this.enabled = false;
            }

            if (hardware != null)
            {
                if (!simulated && hardware.connected && found)
                {
                    rawData = hardware.ports[portIndex].data;
                    DataParse(rawData);
                    OnDataUpdated?.Invoke(rawData);
                }
            }
        }

        public virtual void SVR(float dataFeed)
        {
            
            float.TryParse(minValue, out minMax.x);
            float.TryParse(maxValue, out minMax.y);
            
            svr = Remap(dataFeed, minMax.x, minMax.y, 0f,1f);

        }

        public virtual void SVR(bool dataFeed)
        {
            svr = dataFeed ? 1f : 0f;
        }
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public virtual ModuleType GetModule()
        {
            int number = ModuleNumberQuery();
            foreach (ModuleType module in Enum.GetValues(typeof(ModuleType)))
            {
                if ((int)module == number)
                {
                    return module;
                }
            }

            return ModuleType.None;
        }

        public void AutoSetDataSource()
        {
            gameObject.TryGetComponent(out hardware);

            for (int i = 0; i < hardware.portCount; i++)
            {
                if (moduleNumber == hardware.ports[i].module)
                {
                    portIndex = i;
                    portNumber = i + 1;
                    found = true;
                    return;
                }
            }

            found = false;
        }

        public void DataParse(string stream)
        {
            parsedData = stream.Split(',');
        }

        public void Output(string data)
        {
            string action = "3," + portNumber + "," + data;
            hardware.Output(action);
        }

        public int ModuleNumberQuery()
        {
            return moduleNumber;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Module), true)]
        public class ModuleEditor : Editor
        {
            private Texture2D headerTexture;

            public override void OnInspectorGUI()
            {
                Module module = (Module)target;
                string moduleName = module.GetType().Name.Replace("Module", "");
                string imagePath = $"Materials/{moduleName}";

                headerTexture = Resources.Load<Texture2D>(imagePath);

                if (headerTexture != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(headerTexture, GUILayout.Width(EditorGUIUtility.currentViewWidth / 3), GUILayout.Height(EditorGUIUtility.currentViewWidth / 6));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox($"Image not found: {imagePath}.png", MessageType.Warning);
                }

                // Hide fields if debug is false
                serializedObject.Update();

                if (Module.debug)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    SerializedProperty scriptProp = serializedObject.FindProperty("m_Script");
                    if (scriptProp != null)
                    {
                        EditorGUILayout.PropertyField(scriptProp, true);
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hardware"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("portNumber"), true);
                }

                // Draw the rest of the inspector
                DrawPropertiesExcluding(serializedObject, "m_Script", "hardware", "portNumber");
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
