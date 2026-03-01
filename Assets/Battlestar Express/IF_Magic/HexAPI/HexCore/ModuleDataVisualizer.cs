using UnityEngine;
using System.Collections.Generic;
using Magic;

public class ModuleDataVisualizer : MonoBehaviour
{
    private Hardware device;
    private Dictionary<int, float> portData = new Dictionary<int, float>();
    private bool debug = false;

    void Start()
    {
        device = FindObjectOfType<Hardware>(); // Assuming there's only one Device in the scene
        if (device == null)
        {
            Debug.LogError("Device not found in the scene.");
        }
    }

    void Update()
    {
        if (device != null && device.ports != null)
        {
            foreach (var port in device.ports)
            {
                if (port != null && !string.IsNullOrEmpty(port.data))
                {
                    if (float.TryParse(port.data, out float data))
                    {
                        portData[port.module] = data;
                        if (debug)
                            Debug.Log($"Port {port.module}: Data = {data}");
                    }
                    else
                    {
                        if (debug)
                            Debug.LogWarning($"Port {port.module}: Unable to parse data.");
                    }
                }
            }
        }
    }

    public float GetPortData(int portNumber)
    {
        if (portData.ContainsKey(portNumber))
        {
            return portData[portNumber];
        }
        return 0.0f;
    }

    public float GetPortMinValue(int portNumber)
    {
        // Implement logic to get minimum value for the port
        return 0.0f;
    }

    public float GetPortMaxValue(int portNumber)
    {
        // Implement logic to get maximum value for the port
        return 100.0f;
    }
}
