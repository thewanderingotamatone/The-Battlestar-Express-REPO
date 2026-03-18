using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic
{
    [System.Serializable]
    public class Hardware : MonoBehaviour
    {
        public string dataStream;

        [HideInInspector]
        public int portCount = 8;
        [HideInInspector]
        public List<Port> ports;
        [HideInInspector]
        public State state;

        public bool connected = false;

        public bool ManuallySetSource = false;
        [ConditionalHide("ManuallySetSource")]
        public Magic.Stream source;

        public enum Modes { stream, beacon };
        [SerializeField] public Modes mode = Modes.stream;
        public bool setMode = false;

        private Dictionary<int, int> modes = new Dictionary<int, int>(){
            {0, 1},
            {1, 7}
        };

        public Hardware()
        {
            source = null;
        }

        public Hardware(Stream dataSource)
        {
            source = dataSource;
            ManuallySetSource = true;
        }

        // TODO:
        // Hardware(Gateway source, int id){}
        // Hardware(Cloud source, int id){}

        // find and connect hardware to associated stream
        public virtual void Awake()
        {
            if (!ManuallySetSource)
            {
                AutoSetDataSource();
            }

            ports = PortGenerate(portCount);

            source.Bond(); // call stream start function

            connected = source.connected;

            // parse data once to set hardware
            dataStream = source.rawData;
            DataParse(dataStream);
        }

        // parse data every frame
        public virtual void Update()
        {
            dataStream = source.rawData;
            DataParse(dataStream);

            if (setMode){
                SetMode(mode);
                setMode = false;
            }
        }

        // find first game object with stream script and set it equal to source
        public void AutoSetDataSource()
        {
            Magic.Stream[] sources = GameObject.Find("Nexus").GetComponentsInChildren<Stream>();
            source = sources[0];
        }

        // generate a list of ports based upon hardware
        public List<Port> PortGenerate(int portCount)
        {
            List<Port> ports = new List<Port>();
            for (int i = 0; i < portCount; i++)
            {
                ports.Add(new Port(i + 1));
            }
            return ports;
        }

        // parse universal data model
        public void DataParse(string stream)
        {
            string[] parsedData;
            parsedData = stream.Split(';');

            // parse individual port data
            for (int i = 0; i < portCount; i++)
            {
                if (parsedData.Length > i)
                {
                    string[] miniData = parsedData[i].Split(':');
                    if (miniData.Length > 0)
                    {
                        if (int.TryParse(miniData[0], out ports[i].module)) { };
                    }
                    if (miniData.Length >= 1)
                    {
                        ports[i].data = miniData[1];
                    }
                }
            }

            // TODO: battery status - [8]

            // parse state object
            if (parsedData.Length >= 9)
            {
                state.orientation = parsedData[9];
            }
        }

        // send output data to hardware data stream
        public void Output(string action)
        {
            source.Output(action);
        }

        // set mode on hardware
        public void SetMode(Modes mode){
            source.Output("7,7," + modes[(int)mode]);
        }
    }

}
