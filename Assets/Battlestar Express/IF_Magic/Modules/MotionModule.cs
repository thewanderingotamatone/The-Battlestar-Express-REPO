using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class MotionModule : Module
    {
        // property
        [Header("Properties")]
        public bool movement = false;

        public override void Start()
        {
            moduleNumber = 18;
            dataType = DataTypes.Bool;

            dataLength = 1;
            minValue = "0";
            maxValue = "1";

            base.Start();
        }

        // parse data and set property values
        public override void Update()
        {
            base.Update();

            if (simulated)
            {
                parsedData[0] = Mathf.RoundToInt(Mathf.PingPong((Time.time), 1)).ToString();
            }


            if (parsedData[0] == "1")
            {
                movement = true;
            }
            else
            {
                movement = false;
            }
            SVR(movement);
        }
    }
}

