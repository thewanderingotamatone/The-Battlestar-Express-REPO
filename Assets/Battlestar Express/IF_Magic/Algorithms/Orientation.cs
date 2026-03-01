using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Algorithms
{
    [ExecuteAlways]

    public class Orientation : Algorithm
    {

        private string[] orientationData;

        public Quaternion orientation = new Quaternion();
        public Vector3 acceleration = new Vector3();

        public bool mirror = false;

        public override void Update()
        {
            base.Update();


            orientationData = parsedData[0].Split(':');

            string _testOrientation = "";
            if (orientationData.Length > 1)
            {
                _testOrientation = orientationData[1];
            }

            string _testAcceleration = "";
            if (orientationData.Length > 6)
            {
                _testAcceleration = orientationData[5];
            }

            if (orientationData.Length > 6 && _testOrientation != "05" && _testAcceleration != "-0.01" && _testAcceleration != "118.74")
            {

                float.TryParse(orientationData[3], out orientation.w);
                float.TryParse(orientationData[1], out orientation.x);
                float.TryParse(orientationData[4], out orientation.y);
                float.TryParse(orientationData[2], out orientation.z);

                float.TryParse(orientationData[5], out acceleration.x);
                float.TryParse(orientationData[6], out acceleration.y);
                float.TryParse(orientationData[7], out acceleration.z);

            }

            if (mirror)
            {
                transform.rotation = orientation;
            }


        }
        

    }
}

