using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Magic
{
    public class NamespaceExpansion : MonoBehaviour
    {


    }

    [System.Serializable]
    public class DevicePorts
    {
        // Initialize the array with a size of 8
        public ModuleType[] portModules = new ModuleType[8];

        // Method to get port label
        public string GetPortLabel(int index)
        {
            return (index + 1).ToString();
        }
    }



    public enum ModuleType // alphabetical order
    {
        None = 0,
        Button = 1,
        Color = 4,
        Dial = 2,
        Digital = 10,
        Distance = 8,
        Environment = 21,
        Flex = 19,
        Force = 20,
        Gesture = 3,
        Glow = 9,
        Joystick = 6,
        Light = 11,
        Motion = 18,
        Move = 14,
        Proximity = 5,
        Slider = 15,
        Sound = 12,
        Spin = 7,
        Thermal = 13,
        Tone = 17,
        Touch = 16
    }
    public enum ModuleTypeSVR // alphabetical order
    {
        Button = 1,
        Dial = 2,
        Digital = 10,
        Distance = 8,
        Flex = 19,
        Force = 20,
        //Joystick = 6,
        Light = 11,
        Motion = 18,
        Proximity = 5,
        Slider = 15,
        Sound = 12,
        Spin = 7,
        Thermal = 13,
        Touch = 16
    }


}


