using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class ToneModule : Module
    {
        // property
        [Header("Actions")]
        [Range(0, 3000)]
        public int frequency = 0;
        public int time = 1000;

        // TODO: make button
        public bool setTone = false;

        public override void Start()
        {
            moduleNumber = 17;
            base.Start();
        }

        // send tone data
        public void SetTone(int frequency, int time)
        {
            base.Output(moduleNumber + "," + frequency.ToString() + "," + time.ToString());
        }

        // parse data and set property values
        public override void Update()
        {
            base.Update();

            if (setTone)
            {
                SetTone(frequency, time);
                setTone = false;
            }

        }
    }
}

