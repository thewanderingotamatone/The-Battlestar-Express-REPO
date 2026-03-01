using UnityEngine;
using System.Collections.Generic;
using Magic;

[CreateAssetMenu(fileName = "IFMagicSettings", menuName = "Scriptable Objects/IFMagicSettings")]
public class IFMagicSettings : ScriptableObject
{
    [Header("IFMix User Settings")]
    [Tooltip("Are we playing with IFMix?")]
    public bool usingIFMIX = false;
    
    [Tooltip("Are we playing with more than one IFMix?")]
    [ConditionalHide("usingIFMIX", true)]
    public bool usingMultipleIFMIX = false;

    [Tooltip("Debug to Canvas?")]
    public bool onScreenDebug = false;

    [ConditionalHide(new string[] { "usingIFMIX", "usingMultipleIFMIX" }, new bool[] { true, false }, true)]
    public IfMagicDeviceConfig deviceConfiguration;

    [ConditionalHide("usingMultipleIFMIX", true)]
    public List<IfMagicDeviceConfig> deviceConfigurations;

    [System.Serializable]
    public class IfMagicDeviceConfig
    {
        [Tooltip("The Device Name")]
        public string name = "";

        [Tooltip("The COM Port Local")]
        public int COMPort;

        [Tooltip("The Current Device Ports")]
        public DevicePorts portConfiguration;

        [Tooltip("Stored Null Calibration Direction to PC")]
        public Vector3 calibrationDirection;
    }
    
    [System.Serializable]
    public class DevicePorts
    {
        // Initialize the array with a size of 8
        public ModuleType[] portModules = new ModuleType[8];
    }


    void OnEnable()
    {
        // Initialize lists to avoid null reference issues
        if (deviceConfigurations == null && usingMultipleIFMIX)
        {
            deviceConfigurations = new List<IfMagicDeviceConfig>();
        }
    }
    // void OnGUI{

    // }
}
