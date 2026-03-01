using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Magic
{
    [ExecuteAlways]
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

        private bool found = false;

        public virtual void Start()
        {
            FindHardware();
        }

        void FindHardware()
        {
            if (!ManuallySetSource)
            {
                AutoSetDataSource();
            }
        }

        public virtual void Update()
        {
            if (!found)
            {
                FindHardware();
            }
            else
            {
                rawData = hardware.state.orientation;
                DataParse(rawData);
            }

        }

        // find first hardware game object
        public void AutoSetDataSource()
        {
            gameObject.TryGetComponent<Hardware>(out hardware);

            if (hardware != null)
            {
                found = true;
            }
            else
            {
                found = false;
                Debug.Log("cant find hardware");
            }
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

