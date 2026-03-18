using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Magic
{
    [System.Serializable]
    public class Algorithm : MonoBehaviour
    {
        // public bool simulated;

        public bool ManuallySetSource = false;
        [ConditionalHide("ManuallySetSource")]
        public Hardware hardware;

        private string rawData;
        [HideInInspector]
        public string[] parsedData;

        public virtual void Start()
        {
            if (!ManuallySetSource)
            {
                AutoSetDataSource();
            }
        }

        public virtual void Update()
        {
            rawData = hardware.state.orientation;
            DataParse(rawData);
        }

        // find first hardware game object
        public void AutoSetDataSource()
        {
            hardware = GameObject.FindObjectsOfType<Hardware>()[0];
        }

        // parse data
        public void DataParse(string stream)
        {
            parsedData = stream.Split('<');
        }
        
        // send state data
        public void Output(string data)
        {
            string action = "2," + data;
            hardware.Output(action);
        }

    }
}

