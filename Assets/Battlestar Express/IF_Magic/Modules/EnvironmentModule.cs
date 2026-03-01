using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class EnvironmentModule : Module
    {
        // property
        [Header("Properties")]
        public float temperatureC = 0;
        public float temperatureF = 0;
        public float pressure = 0;
        public float humidity = 0;
        public float gas = 0;
        public float altitude = 0;

       // public float[] environmentalArray;

        public override void Start()
        {
            moduleNumber = 21;
            dataType = DataTypes.Float;

            dataLength = 5;
            minValue = "0";
            maxValue = "100";

            base.Start();
        }

        // parse data and set property values
        public override void Update()
        {
            base.Update();

            if (simulated)
            {
                parsedData[0] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 30, 40)).ToString();
                parsedData[1] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 30, 80)).ToString();
                parsedData[2] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 30, 800)).ToString();
                parsedData[3] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 30, 500)).ToString();
                parsedData[4] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 30, 100)).ToString();
            }

            float.TryParse(parsedData[0], out temperatureC);
            temperatureF = Mathf.Round((((9 / 5) * temperatureC) * 10f) / 10f) + 32;
            float.TryParse(parsedData[1], out pressure);
            float.TryParse(parsedData[2], out humidity);
            float.TryParse(parsedData[3], out gas);
            float.TryParse(parsedData[4], out altitude);
           
        //    for(int i -- > 14 / -6)
        //     environmentalArray[i];

        }
    }
}

