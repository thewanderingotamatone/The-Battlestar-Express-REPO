using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Magic
{
    [System.Serializable]
    public class Module : MonoBehaviour
    {
        protected int moduleNumber;

        public bool simulated = false;

        public bool ManuallySetSource = false;
        [ConditionalHide("ManuallySetSource")]
        public Hardware hardware;
        [ConditionalHide("ManuallySetSource")]
        public int portNumber;

        private int portIndex;

        private bool found = false;

        // variables to keep track of data
        private string rawData;
        [HideInInspector]
        public string[] parsedData;
        [HideInInspector]
        public string[] processedData;

        [HideInInspector]
        public int dataLength; // variable to set data array length

        // variable for data range TODO: make array to set individual property maxes
        [HideInInspector]
        public string minValue;
        [HideInInspector]
        public string maxValue;

        public enum DataTypes { String, Integer, Float, Bool };
        [HideInInspector]
        public DataTypes dataType;


        // find port module is connected to
        public virtual void Start()
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
        }

        // read data every frame
        public virtual void Update()
        {
            if (hardware != null)
            {
                if (!simulated && hardware.connected && found)
                {
                    rawData = hardware.ports[portIndex].data;
                    DataParse(rawData);
                }
            }
        }

        // find first hardware game object and find the port number with matching module number and set as index
        // if not found set found as false
        public void AutoSetDataSource()
        {
            hardware = GameObject.FindObjectsOfType<Hardware>()[0];

            for (int i = 0; i < hardware.portCount; i++)
            {
                if (moduleNumber == hardware.ports[i].module)
                {
                    portIndex = i;
                    portNumber = i + 1;
                    found = true;
                    break;
                }
            }
        }

        // split module data into individual components
        public void DataParse(string stream)
        {
            parsedData = stream.Split(',');
        }

        // send module signal data to hardware
        public void Output(string data)
        {
            string action = "3," + portNumber + "," + data;
            hardware.Output(action);
        }

    }
}

