using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class ProximityModule : Module
    {
        // property
        [Header("Properties")]
        public int raw = 0;
        public int amount = 0;

        public override void Start()
        {
            moduleNumber = 5;
            dataType = DataTypes.Integer;

            dataLength = 1;
            minValue = "0";
            maxValue = "255";

            base.Start();
        }

        // parse data and set property values
        public override void Update()
        {
            base.Update();

            if (simulated)
            {
                parsedData[0] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 10, 255)).ToString();
            }

            int.TryParse(parsedData[0], out raw);
            amount = (int)((raw / 255f) * (100f));
        }
    }
}

