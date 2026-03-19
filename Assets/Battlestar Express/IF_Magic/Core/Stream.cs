using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Magic
{
    public class Stream : MonoBehaviour
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern void SendMessageToUnity(string message);
        #endif
        // stream status
        // [HideInInspector]
        public string data = "";
        public bool connected = false;

        // connection configurations
        [Header("WIRED")]
        public int serialNumber = 0;
        [Header("WIRELESS")]
        public bool wireless = false;
        [ConditionalHide("wireless")]
        public string hardwareName = "Device";

        // stream parameters
        private int baudRate = 115200;
        private SerialPort port;
        private Thread streamThread;
        private Queue<string> outputQueue = new Queue<string>();
        private bool isProcessingQueue = false;
        private float portUpdateDelay = 4.0f;


        // setup up a serial connection with hardware
        public void Bond()
        {
            if (connected) return;
            if (Connect())
            {
                if (streamThread == null)
                {
                    port.DiscardInBuffer(); // flush buffer
                    streamThread = new Thread(DataStream);
                    streamThread.Start();
                    connected = true;
                    Debug.Log("Thread started");
                }
            }
            else
            {
                Debug.LogWarning("Hardware is not connected or powered on");
            }

        }
       public void BondForce()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                return;
            #endif
            //connected = false;
            if (Connect())
            {
                if (streamThread == null)
                {
                    port.DiscardInBuffer(); // flush buffer
                    streamThread = new Thread(DataStream);
                    streamThread.Start();
                    connected = true;
                    Debug.Log("Thread started");
                }
            }
            else
            {
                Debug.LogWarning("Hardware is not connected or powered on");
            }

        }
        #if UNITY_WEBGL && !UNITY_EDITOR
        public void ReceiveData(string _data)
        {
            //connected = true;
            Debug.Log("Data received from JavaScript: " + data);
            data = _data;
        }
        // Method to be called from JavaScript p.II
        public void ConnectedState(int checkConnection)
        {
            bool isConnected = false;
            if (checkConnection == 1) isConnected = true;
            Debug.Log("Connection State Updated : " + isConnected);
            connected = isConnected;
        }
        #endif
        // connect to hardware
        private bool Connect()
        {
            try
            {
                port = new SerialPort(SetPathName(StringifyPathNumber(serialNumber), wireless), baudRate);
                port.Open();
                return true;
            }
            catch
            {
                return COMPortAutodetectCatch(); // recursively check ports from autodetect options
            }
        }
        private bool Connect(int attemptedPort)
        {
            try
            {
                port = new SerialPort(SetPathName(StringifyPathNumber(attemptedPort), wireless), baudRate);
                port.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        string StringifyPathNumber(int numberToString)
        {
            if (serialNumber < 1) // sets to null for macs without port declared, -1 default
            {
                return "";
            }
            else
            {
                return "" + numberToString;
            }
        }

        // Autogenerate Modules as components

        bool COMPortValid()
        {


            //TestComPort(port);
            // if connect to bluetooth, fail, break, add port -- add timeout.. for failed / hardware not connected? Manual 
            // if one comes back right, the others are cut

            //"1,..2,"
            try
            {
                port = new SerialPort(SetPathName(StringifyPathNumber(serialNumber), wireless), baudRate);
                port.Open();
                return true;
            }
            catch{
                return false;
            }

            // if fails, debug error return false 

        }

        bool COMPortAutodetectCatch()
        {

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                string[] ports = SerialPort.GetPortNames();
                List<int> numbers = new List<int>();

                foreach (string port in ports)
                {
                    numbers.Add(int.Parse(port.Split('M')[1]));
                }


                foreach (int trialNumber in numbers)
                {
                    if (Connect(trialNumber))
                    {
                        serialNumber = trialNumber;
                        Debug.Log("Found potential port number at : " + trialNumber);

                        if (COMPortValid())
                        {
                            return true;
                        }
                    }
                }
            }
            else // on a mac
            {
                //
                // return Connect();
            } // incremenet here through numbers, try connect again.s


            return false;
        }




        // disconnect from hardware
        public void Disconnect()
        {
            Debug.Log("disconnecting");
            if (port.IsOpen)
            {
                port.Close();
            }
            streamThread?.Abort();
            connected = false;
        }

        // read data every frame
        public void DataStream()
        {
            while (true)
            {
                if (port.IsOpen)
                {
                    data = port.ReadLine();
                }
            }
        }


        // send data over serial to hardware
        public void Output(string message)
        {
            
            lock (outputQueue)
            {
                outputQueue.Enqueue(OutputSanitize(message));
                if (!isProcessingQueue)
                {
                    isProcessingQueue = true;
                    StartCoroutine(ProcessOutputQueue());
                }
            }
        }

        private IEnumerator ProcessOutputQueue()
        {
            while (outputQueue.Count > 0)
            {
                string message = outputQueue.Dequeue();
                Debug.Log(message);
               port.Write(message);
                yield return new WaitForSeconds(portUpdateDelay); // Delay of 1 second between each write
            }
            isProcessingQueue = false;
        }

        private string OutputSanitize(string potentialOutput)
        {

            // Need viable list of commands from lance to process into common english

            // Debug.Log(hey, that was a bad command)    //TODO 
            //dont let me brick myself
            // not allow, extra strings at once
            // double check bus - incoming commands not full,
            // number of arguments / , sub X max num

            // 1, 4, 5 -- corresponds to ____ command
            // turn into english
            // english -> command
            // Commented out sanitization logic:
            // List<string> validCommands = new List<string> { "1,*", "2,*", "3,*" };
            // bool isValid = validCommands.Any(valid => potentialOutput.StartsWith(valid));
            // if (!isValid)
            // {
            //     Debug.Log("Invalid command: " + potentialOutput);
            //     return string.Empty;
            // }

            return potentialOutput;
        }

        // disconnect serial port and stop thread when closed
        void OnApplicationQuit()
        {
            Connect();
        }

        // generate serial port path name based upon platform and wireless setting
        public string SetPathName(string number, bool wireless)
        {
            string path = "";

            // Mac Editor
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (wireless)
                {
                    // EXAMPLES:
                    // /dev/cu.Device-ESP32SPP
                    // /dev/cu.Device-ESP32SPP-1
                    // /dev/cu.Device-DS
                    path = "/dev/cu." + hardwareName + "-ESP32SPP-" + number;

                }
                else
                {
                    // EXAMPLES:
                    // /dev/cu.SLAB_USBtoUART
                    // /dev/cu.SLAB_USBtoUART7
                    path = "/dev/cu.SLAB_USBtoUART" + number;
                }
            }

            // Windows Editor
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //wired + wireless
                path = "COM" + (number == "" ? "5" : number);
            }

            // Mac Player
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                if (wireless)
                {
                    path = "/dev/cu." + hardwareName + "-ESP32SPP";
                }
                else
                {
                    path = "/dev/cu.SLAB_USBtoUART" + number;
                }
            }

            // Windows Player
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                //wired + wireless
                path = "COM" + (number == "" ? "3" : number);
            }

            return path;
        }


        // Editor 
        // call Bond()

        //Parse the stream, one stream element 5,sdfasdfdas (check the data is really us)
        // 8 sections 
        // data
        // each 8 sections  
        // " module number, : 
        // true for a given module, true for all modules/devices
        // ___ : 456, (1)
        //  1 ; 2 ; ... 8
        // MODNUM : DATA ; MODNUM2 : DATA
        // give me the MODNUM equivalences as modules
        // eg if i see 20, i know slider
        // 

    }
    #if UNITY_EDITOR
    [CustomEditor(typeof(Stream))]
    public class StreamEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Stream script = (Stream)target;
            if (GUILayout.Button("Connect"))
            {
                script.BondForce();
            }
        }
    }
    #endif
}






