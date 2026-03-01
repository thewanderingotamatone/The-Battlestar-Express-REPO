using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Magic;

[ExecuteAlways]
public class PhysicalModuleAnalog : MonoBehaviour
{
    private IFMixer ifMixer;

    void OnEnable()
    {
        // Find the IFMixer object in the scene
        ifMixer = FindObjectOfType<IFMixer>();

        if (ifMixer == null)
        {
            Debug.LogError("IFMixer object not found in the scene.");
        }
        else
        {
            ifMixer.OnPortConfigurationChanged += UpdateModules;
        }

        // Initial update
        UpdateModules();
    }

    void OnDisable()
    {
        if (ifMixer != null)
        {
            ifMixer.OnPortConfigurationChanged -= UpdateModules;
        }
    }

    void Start()
    {
        UpdateModules();
    }

    private void UpdateModules()
    {
        if (ifMixer != null)
        {
            // Loop through each port's data
            for (int i = 0; i < ifMixer.parsedPortsData.Count; i++)
            {
                if (i < 8) // Only update modules for ports 1-8
                {
                    ModuleType moduleType = ifMixer.parsedPortsData[i].Module;
                    Transform portTransform = transform.GetChild(i);

                    if (portTransform != null)
                    {
                        // Loop through each child and activate the correct module
                        for (int j = 0; j < portTransform.childCount; j++)
                        {
                            Transform moduleTransform = portTransform.GetChild(j);
                            string moduleName = moduleType.ToString();

                            if (moduleType == ModuleType.None)
                            {
                                moduleTransform.gameObject.SetActive(false);
                            }
                            else if (moduleTransform.name == moduleName)
                            {
                                moduleTransform.gameObject.SetActive(true);
                            }
                            else if (moduleTransform.name.Contains("[Foundation]"))
                            {
                                moduleTransform.gameObject.SetActive(true);
                                // Update the module type in the ModulePedestalController
                                ModulePedestalController pedestalController = moduleTransform.GetComponent<ModulePedestalController>();
                                if (pedestalController != null)
                                {
                                    pedestalController.module = moduleType;
                                    pedestalController.PedestalRename();
                                }
                            }
                            else
                            {
                                moduleTransform.gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
}
