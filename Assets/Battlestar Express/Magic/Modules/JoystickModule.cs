using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class JoystickModule : Module
    {
        // property
        [Header("Properties")]
        public int rawX = 0;
        public int rawY = 0;
        public int x = 0;
        public int y = 0;

        public override void Start()
        {
            moduleNumber = 6;
            dataType = DataTypes.Integer;

            dataLength = 2;
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
                parsedData[0] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 30, 4095)).ToString();
                parsedData[1] = Mathf.RoundToInt(Mathf.PingPong((Time.time) * 30, 4095)).ToString();
            }

            int.TryParse(parsedData[0], out rawX);
            int.TryParse(parsedData[1], out rawY);
            x = (int)((rawX / 4095f) * (100f));
            y = (int)((rawY / 4095f) * (100f));
        }
    }
}

