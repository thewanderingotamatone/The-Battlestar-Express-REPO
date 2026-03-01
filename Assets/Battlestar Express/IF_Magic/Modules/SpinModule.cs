using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class SpinModule : Module
    {
        public enum SpinDirection { still, clockwise, counterclockwise };
        // property
        [Header("Properties")]
        public int rotation = 0;
        [SerializeField] public SpinDirection direction = SpinDirection.still;

        private int[] directionArray = { 0, 0, 0, 0, 0, 0 };

        public override void Start()
        {
            moduleNumber = 7;
            dataType = DataTypes.Integer;

            dataLength = 1;
            minValue = "-1000";
            maxValue = "1000";

            base.Start();
        }

        // parse data and set property values
        public override void Update()
        {
            base.Update();

            if (simulated)
            {
                parsedData[0] = Mathf.RoundToInt(Mathf.Sin((Time.time) / 4) * 1000).ToString();
            }

            int.TryParse(parsedData[0], out rotation); // parse data

            directionArray[1] = directionArray[0];
            directionArray[2] = directionArray[1];
            directionArray[3] = directionArray[2];
            directionArray[4] = directionArray[3];
            directionArray[5] = directionArray[4];
            directionArray[0] = rotation;

            if (directionArray[0] > directionArray[5])
            {
                direction = SpinDirection.clockwise;
            }
            else if (directionArray[0] < directionArray[5])
            {
                direction = SpinDirection.counterclockwise;
            }
            else
            {
                direction = SpinDirection.still;
            }
            SVR(rotation);
        }
    }
}

