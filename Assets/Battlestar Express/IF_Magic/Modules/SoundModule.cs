using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class SoundModule : Module
    {
        // property
        [Header("Properties")]
        public int volume = 0;

        public override void Start()
        {
            moduleNumber = 12;
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

            int.TryParse(parsedData[0], out volume);
            SVR(volume);
        }
    }
}

