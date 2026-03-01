using System.Collections.Generic;
using UnityEngine;
using System;

using Magic;

namespace Magic
{
    [ExecuteAlways]
    [System.Serializable]
    public class Hardware : MonoBehaviour
    {
        // [HideInInspector]
        public string data;

        // hardware properties
        [HideInInspector]
        public int portCount = 8;
        [HideInInspector]
        public List<Port> ports;
        [HideInInspector]
        public State state;

        // stream properties
        [Header("STREAM")]
        public bool connected = false;
        public bool ManuallySetSource = false;
        [ConditionalHide("ManuallySetSource")]
        public Magic.Stream source;


        public enum Modes { stream, beacon };
        [Header("METHODS")]
        [SerializeField] public Modes mode = Modes.stream;
        public bool setMode = false;

        public DevicePorts devicePorts;

        private Dictionary<int, int> modes = new Dictionary<int, int>(){
            {0, 1},
            {1, 7}
        };

        public Hardware()
        {
            source = null;
        }

        public Hardware(Stream stream)
        {
            source = stream;
            ManuallySetSource = true;
        }

        // parse data every frame
        public virtual void Update()
        {
            // find and connect hardware to associated stream
            if (!ManuallySetSource)
            {
                AutoSetDataSource();
            }

            if (source != null)
            {
                source.Bond();

                ports = GeneratePorts(portCount);
                connected = source.connected;

                data = source.data;
                Parse(data);

                // set mode button
                if (setMode)
                {
                    SetMode(mode);
                    setMode = false;
                }
            }
        }

        // find stream script and set it equal to source
        public void AutoSetDataSource()
        {
            TryGetComponent<Stream>(out source);
        }

        // generate a list of ports based upon hardware
        public List<Port> GeneratePorts(int portCount)
        {
            List<Port> ports = new();
            for (int i = 0; i < portCount; i++)
            {
                ports.Add(new Port(i + 1));
            }
            return ports;
        }

        // parse universal data model
        public void Parse(string stream)
        {
            string[] data;
            data = stream.Split(';');

            // parse individual port data
            for (int i = 0; i < portCount; i++)
            {
                if (data.Length > i)
                {
                    string[] miniData = data[i].Split(':');
                    if (miniData.Length > 0)
                    {
                        if (int.TryParse(miniData[0], out ports[i].module))
                        {
                            // find the module number 
                            // if, port[i].module button
                            var modSet = GetModuleType(ports[i].module);
                            devicePorts.portModules[i] = modSet;
                            EnableModuleComponent(modSet);

                        }
                    }
                    if (miniData.Length > 1)
                    {
                        ports[i].data = miniData[1];
                    }
                }
            }

            // TODO: battery status - [8]

            // parse state object
            if (data.Length > 9)
            {
                state.orientation = data[9];
            }
        }
        public ModuleType GetModuleType(int number)
        {
            // Iterate through all possible values of ModuleType
            foreach (ModuleType module in Enum.GetValues(typeof(ModuleType)))
            {
                if ((int)module == number)
                {
                    return module;
                }
            }

            // If no matching number is found, return None
            return ModuleType.None;
        }


        public void EnableModuleComponent(ModuleType moduleType)
        {
           // gameObject.GetComponent<Magic.Modules.SliderModule>().enabled = true;
            
            if(moduleType == ModuleType.None)return;
            // Construct the name of the component based on the enum
            string componentName = "Magic.Modules." + moduleType.ToString() + "Module";
            
            // Get the type of the component
            Type componentType = Type.GetType(componentName);

            if (componentType != null)
            {
                // Check if the component is already attached to the GameObject
                Component component = gameObject.GetComponent(componentType);

                if (component == null)
                {
                    // If the component is not found, add it
                    component = gameObject.AddComponent(componentType);
                }

                // Enable the component
                (component as MonoBehaviour).enabled = true;
            }
            else
            {
                Debug.LogError($"Component type '{componentName}' not found. Ensure the script exists and is named correctly.");
            }
        }

        // send output data to hardware data stream
        public void Output(string action)
        {
            source.Output(action);
        }

        // set mode on hardware
        public void SetMode(Modes mode)
        {
            source.Output("7,7," + modes[(int)mode]);
        }

        // set module on port
        public void SetModule(int port, int module)
        {
            source.Output("1," + port + "," + module);
        }
    }

}
