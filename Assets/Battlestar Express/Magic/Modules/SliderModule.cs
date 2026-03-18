using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class SliderModule : Module
    {
        // property
        [Header("Properties")]
        public int raw = 0;
        public int position = 0;

        public override void Start()
        {
            moduleNumber = 15;
            dataType = DataTypes.Integer;

            dataLength = 1;
            minValue = "0";
            maxValue = "4095";

            base.Start();
        }

        // parse data and set property values
        public override void Update()
        {
            base.Update();

            if (simulated)
            {
                parsedData[0] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 20, 4095)).ToString();
            }

            int.TryParse(parsedData[0], out raw);
            position = (int)((raw / 4095f) * (100f));
        }
    }
}

