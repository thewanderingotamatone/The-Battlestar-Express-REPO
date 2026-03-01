using System.Collections.Generic;
using UnityEngine;
using Magic;
using UnityEditor;

public class ModuleManager : MonoBehaviour
{
    public List<ModuleType> portConfigurations = new List<ModuleType>(new ModuleType[8]);
    public ModuleDataVisualizer dataVisualizer;
    public bool showRawValues;

    private bool debug = false;
    private Stream stream;
    private IFMagicSettings settings;

    private const string settingsPath = "Assets/Settings/IFMagicSettings.asset";

    private void OnEnable()
    {
        LoadSettings();
        if (settings != null)
        {
            UpdatePortConfigurationsFromSettings(settings.deviceConfiguration.portConfiguration.portModules);
        }
    }

    private void Start()
    {
        if (Application.isPlaying && settings != null)
        {
            UpdatePortConfigurationsFromSettings(settings.deviceConfiguration.portConfiguration.portModules);
        }
    }

    private void OnDisable()
    {
        if (!Application.isPlaying && settings != null)
        {
            settings.deviceConfiguration.portConfiguration.portModules = portConfigurations.ToArray();
            #if UNITY_EDITOR
            EditorUtility.SetDirty(settings); // Mark the settings object as dirty to ensure changes are saved
            AssetDatabase.SaveAssets(); // Save the changes to the asset file
            #endif
        }
    }

    private void LoadSettings()
    {
        #if UNITY_EDITOR
        settings = AssetDatabase.LoadAssetAtPath<IFMagicSettings>(settingsPath);
        #endif
        if (settings == null)
        {
            Debug.LogError("Failed to load settings from path: " + settingsPath);
        }
    }

    public void ModuleChange(int portNumber, int moduleCodeNumber)
    {
        string outputCode = "1," + portNumber + "," + moduleCodeNumber;
        stream = FindObjectOfType<Stream>();
        stream.Output(outputCode);
        if (debug) Debug.Log(stream + "  : " + outputCode);
    }

    [ContextMenu("Update Modules")]
    public void UpdateModules()
    {
        for (int i = 0; i < portConfigurations.Count; i++)
        {
            ModuleType moduleType = portConfigurations[i];
            if (moduleType != ModuleType.None)
            {
                if (debug) Debug.Log(moduleType);
                int moduleCodeNumber = (int)moduleType;
                ModuleChange(i + 1, moduleCodeNumber);
            }
        }
        for (int i = 0; i < portConfigurations.Count; i++) // none overwrites
        {
            ModuleType moduleType = portConfigurations[i];
            if (moduleType == ModuleType.None)
            {
                if (debug) Debug.Log(moduleType);
                int moduleCodeNumber = (int)moduleType;
                ModuleChange(i + 1, moduleCodeNumber);
            }
        }

        // Update the settings configuration to mirror the current port configurations
        if (settings != null)
        {
            settings.deviceConfiguration.portConfiguration.portModules = portConfigurations.ToArray();
            #if UNITY_EDITOR
            EditorUtility.SetDirty(settings); // Mark the settings object as dirty to ensure changes are saved
            AssetDatabase.SaveAssets(); // Save the changes to the asset file
            #endif
        }
    }

    public void UpdatePortConfigurationsFromSettings(ModuleType[] portModules)
    {
        portConfigurations = new List<ModuleType>(portModules);
    }
}
