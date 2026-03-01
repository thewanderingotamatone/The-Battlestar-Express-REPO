using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class ColorModule : Module
    {
        // property
        [Header("Properties")]
        public Color color = new Color(0, 0, 0);
        public int alpha;
        public int red;
        public int green;
        public int blue;


        private int rawAlpha;
        private int rawRed;
        private int rawGreen;
        private int rawBlue;

        public override void Start()
        {
            moduleNumber = 4;
            dataType = DataTypes.Integer;

            dataLength = 4;
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
                parsedData[0] = "255";
                parsedData[1] = Mathf.RoundToInt(Mathf.PingPong((Time.time), 255)).ToString();
                parsedData[2] = Mathf.RoundToInt(Mathf.PingPong((Time.time), 255)).ToString();
                parsedData[3] = Mathf.RoundToInt(Mathf.PingPong((Time.time), 255)).ToString();
            }


            if (parsedData.Length >1)
            {
            int.TryParse(parsedData[0], out alpha);
            int.TryParse(parsedData[1], out red);
            int.TryParse(parsedData[2], out green);
            int.TryParse(parsedData[3], out blue);

            // color.a = 255;
            if (alpha != 0)
            {
                float r = ((float)red / (float)alpha);
                float g = ((float)green / (float)alpha);
                float b = ((float)blue / (float)alpha);


                color = new Color(r, g, b);

                // red = (int)(((float)rawRed / rawAlpha) * 255);
                // green = (int)(((float)rawGreen / rawAlpha) * 255);
                // blue = (int)(((float)rawBlue / rawAlpha) * 255);
            }
            }

        }
    }
}

