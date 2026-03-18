using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class GlowModule : Module
    {

        [Header("Actions")]
        public UnityEngine.Color color;

        // TODO: make button
        public bool setColor = false;

        public override void Start()
        {
            moduleNumber = 9;
            base.Start();
        }

        // send color data
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
            base.Update();

            if (setColor)
            {
                SetColor(color);
                setColor = false;
            }
        }
    }
}

