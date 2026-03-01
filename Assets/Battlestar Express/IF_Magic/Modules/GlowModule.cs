using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class GlowModule : Module
    {

        [Header("Actions")]
        public UnityEngine.Color[] color = new UnityEngine.Color[6];

        // TODO: make button
        public bool setColor = false;
        public bool specialColor = false;


        // send color data
        public void SetColor(UnityEngine.Color[] colors)
        {

            float H, S, V;
            string color = moduleNumber + ",";

            for (int i = 0; i < 6; i++)
            {
                Color.RGBToHSV(colors[i], out H, out S, out V);
                int hue, saturation, value;

                hue = Mathf.RoundToInt(H * 255);
                saturation = Mathf.RoundToInt(S * 255);
                value = 255 - Mathf.RoundToInt(V * 255);


                color = color + i + "," + (i + 1) + ",0," + hue + "," + saturation + ":1," + value + (i == 5 ? "" : "-");
            }
            base.Output(color);

        }

        
        
        public void SetColor(UnityEngine.Color color)
        {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            int hue, saturation, value;

            hue = Mathf.RoundToInt(H * 255);
            saturation = Mathf.RoundToInt(S * 255);
            value = 255 - Mathf.RoundToInt(V * 255);
            base.Output(moduleNumber + ",0,6,0," + hue + "," + saturation + ":1," + value);
        }

        public override void Update()
        {
            base.moduleNumber = 9;
            base.Update();

            if (setColor)
            {
                SetColor(color);
                setColor = false;
            }
            if(specialColor)
            {
                string colorString = "9,0,6,2,40"; // 40 is the speed
                base.Output(colorString);
                SetColor(Color.black);
                specialColor = false;
            }

            
        }
    }

}




