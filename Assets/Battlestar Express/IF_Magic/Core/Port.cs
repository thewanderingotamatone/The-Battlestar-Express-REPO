using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic
{
    [System.Serializable]
    public class Port
    {
        public int number = -1;
        public int module = -1;
        public string data = "";

        public Port(int portNumber)
        {
            number = portNumber;
        }
    }

    [System.Serializable]
    public class State
    {
        public string orientation;
    }
}