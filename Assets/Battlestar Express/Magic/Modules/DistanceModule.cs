using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class DistanceModule : Module
    {
        // property
        [Header("Properties")]
        public float millimeters = 0;
        public float inches = 0;
        public float feet = 0;

        public override void Start()
        {
            moduleNumber = 8;
            dataType = DataTypes.Float;

            dataLength = 1;
            minValue = "0";
            maxValue = "1800";

            base.Start();
        }

        // parse data and set property values
        public override void Update()
        {
            base.Update();

            if (simulated)
            {
                parsedData[0] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 20, 1800)).ToString();
            }

            float.TryParse(parsedData[0], out millimeters);
            inches = Mathf.Round((millimeters / 25.4f) * 10f) / 10f;
            feet = (Mathf.Round((inches / 12f) * 100f) / 100f);
        }
    }
}

