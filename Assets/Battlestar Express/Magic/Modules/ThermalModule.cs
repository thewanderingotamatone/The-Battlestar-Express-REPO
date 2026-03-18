using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class ThermalModule : Module
    {
        // property
        [Header("Properties")]
        public float[] thermalGridC = new float[64];
        public float[] thermalGridF = new float[64];
        public float averageTemperatureC;
        public float centerTemperatureC;
        public float averageTemperatureF;
        public float centerTemperatureF;

        private float totalTemperature = 0;

        public override void Start()
        {
            moduleNumber = 13;
            dataType = DataTypes.Float;

            dataLength = 64;
            minValue = "0";
            maxValue = "80";

            base.Start();
        }

        // parse data and set property values
        public override void Update()
        {
            base.Update();

            if (simulated)
            {
                parsedData[0] = Mathf.RoundToInt(Mathf.PingPong((Time.time), 1)).ToString();

                for (int i = 0; i < 64; i++)
                {
                    parsedData[i] = (Mathf.RoundToInt(Random.Range(-0.2f, 40.0f))).ToString();
                }

            }

            for (int i = 0; i < 63; i++)
            {
                if (float.TryParse(parsedData[i], out thermalGridC[i]))
                {
                    thermalGridF[i] = (Mathf.Round((((9 / 5) * thermalGridC[i]) * 10f) / 10f)) + 32;
                    totalTemperature = thermalGridC[i] + totalTemperature;
                }
            }

            averageTemperatureC = Mathf.Round(((totalTemperature / 64) * 10f)) / 10f;
            averageTemperatureF = (Mathf.Round((((9 / 5) * (totalTemperature / 64)) * 10f)) / 10f) + 32;
            totalTemperature = 0;

            centerTemperatureC = thermalGridC[32];
            centerTemperatureF = (Mathf.Round((((9 / 5) * centerTemperatureC) * 10f) / 10f)) + 32;

        }
    }
}

