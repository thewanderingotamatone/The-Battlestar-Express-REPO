using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class MoveModule : Module
    {
        // property
        [Header("Actions")]
        [Range(0, 180)]
        public int degree = 0;

        // TODO: make button
        public bool setDegree = false;

        public override void Start()
        {
            moduleNumber = 14;
            base.Start();
        }

        // send degree data
        public void SetDegree(int degree)
        {
            base.Output(moduleNumber + "," + degree.ToString());
        }

        public override void Update()
        {
            base.Update();

            if (setDegree)
            {
                SetDegree(degree);
                setDegree = false;
            }

        }
    }
}

