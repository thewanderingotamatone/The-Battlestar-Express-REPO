using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class TouchModule : Module
    {
        // property
        [Header("Properties")]
        public int strength = 0;

        public override void Start()
        {
            moduleNumber = 16;
            dataType = DataTypes.Integer;

            dataLength = 1;
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
                parsedData[0] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 20, 100)).ToString();
            }

            int.TryParse(parsedData[0], out strength);

        }
    }
}

