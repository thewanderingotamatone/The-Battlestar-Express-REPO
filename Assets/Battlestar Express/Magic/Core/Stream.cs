using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;


namespace Magic
{
    public class Stream : MonoBehaviour
    {
        [HideInInspector]
        public string rawData = ""; // data from serial port

        public string portNumber = "";
        public bool wireless = false;
        [ConditionalHide("wireless")]
        public string deviceName = "Device";

        // baud rate is set on hardware
        private int baudRate = 115200;
        private SerialPort port;

        public bool connected = false;

        private Thread streamThread;

        // setup up a serial connection with hardware
        // NOTE: if call in own Awake can't guarantee order
        public void Bond()
        {
            try
            {
                Connect();
            }
            catch
            {
                Debug.LogWarning("Hardware is not connected or powered on");
            }
        }

        void Update() { }

        // read data every frame
        public void DataStream()
        {
            while (true)
            {
                if (connected)
                {
                    rawData = port.ReadLine();
                }
            }
        }

        // send data over serial to hardware
        public void Output(string message)
        {
            port.Write(message);
        }

        // close serial port when closed
        void OnApplicationQuit()
        {
            Disconnect();
        }

        // connect to hardware
        public void Connect()
        {
            port = new SerialPort(SetPathName(portNumber, wireless), baudRate);

            port.Open();
            connected = true;

            // perform an initial read so that hardware can be set
            if (port.IsOpen)
            {
                port.DiscardInBuffer(); // flush buffer
                rawData = port.ReadLine();

                // start seperate thread
                streamThread = new Thread(DataStream);
                streamThread.Start();
            }

        }

        // disconnect from hardware
        public void Disconnect()
        {
            if (port.IsOpen)
            {
                port.Close();
                connected = false;
            }
            streamThread.Abort();
        }

        // generate serial port path name based upon platform and wireless setting
        public string SetPathName(string number, bool wireless)
        {
            //TODO: automatically check to see what ports are connected
            //TODO: switch wireless to BLE

            string portString = "";

            // Mac Editor
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (wireless)
                {
                    // EXAMPLES:
                    // /dev/cu.Device-ESP32SPP
                    // /dev/cu.Device-ESP32SPP-1
                    // /dev/cu.Device-DS
                    // portString = "/dev/cu." + deviceName + "-ESP32SPP-" + number;
                    portString = "/dev/cu." + deviceName + number;

                }
                else
                {
                    // EXAMPLES:
                    // /dev/cu.SLAB_USBtoUART
                    // /dev/cu.SLAB_USBtoUART7
                    portString = "/dev/cu.SLAB_USBtoUART" + number;
                }
            }

            // Windows Editor
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //wired + wireless
                portString = "COM" + number;
            }

            // Mac Player
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                if (wireless)
                {
                    // portString = "/dev/cu." + deviceName + "-ESP32SPP";
                    portString = "/dev/cu." + deviceName;
                }
                else
                {
                    portString = "/dev/cu.SLAB_USBtoUART" + number;
                }
            }

            // Windows Player
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                //wired + wireless
                portString = "COM" + number;
            }

            return portString;
        }
    }
}


