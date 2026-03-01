using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Magic.Algorithms
{
    public class OrientationUngimbled : Algorithm
    {
        private string[] orientationData;

        public Quaternion orientation = new Quaternion();
        public Vector3 acceleration = new Vector3();

        private Quaternion previousOrientation = new Quaternion();
        public float orientationChangeSpeedThreshold = 1.0f; // Threshold for detecting high orientation change speed
        public UnityEvent onHighOrientationChange; // Event to call when high orientation change is detected

        public bool mirror = false;

        public float currentOrientationChangeSpeed; // Public variable to show the current orientation change speed
        public string highestSpeedAxis; // Public variable to show which axis has the highest speed of change
        public float runningMaximumSpeed; // Public variable to show the running maximum of the highest speed throughout the game

        public override void Start()
        {
            base.Start();
            previousOrientation = orientation;
            runningMaximumSpeed = 0f;
        }

        public override void Update()
        {
            base.Update();

            orientationData = parsedData[0].Split(':');

            string _testOrientation = orientationData[1];
            string _testAcceleration = orientationData[5];

            if (_testOrientation != "05" && _testAcceleration != "-0.01" && _testAcceleration != "118.74")
            {
                float.TryParse(orientationData[3], out orientation.w);
                float.TryParse(orientationData[1], out orientation.x);
                float.TryParse(orientationData[4], out orientation.y);
                float.TryParse(orientationData[2], out orientation.z);

                float.TryParse(orientationData[5], out acceleration.x);
                float.TryParse(orientationData[6], out acceleration.y);
                float.TryParse(orientationData[7], out acceleration.z);

                // Calculate orientation change speed for each axis
                float speedX = Mathf.Abs(orientation.x - previousOrientation.x) / Time.deltaTime;
                float speedY = Mathf.Abs(orientation.y - previousOrientation.y) / Time.deltaTime;
                float speedZ = Mathf.Abs(orientation.z - previousOrientation.z) / Time.deltaTime;

                // Determine the highest speed axis
                if (speedX > speedY && speedX > speedZ)
                {
                    currentOrientationChangeSpeed = speedX;
                    highestSpeedAxis = "X";
                }
                else if (speedY > speedX && speedY > speedZ)
                {
                    currentOrientationChangeSpeed = speedY;
                    highestSpeedAxis = "Y";
                }
                else
                {
                    currentOrientationChangeSpeed = speedZ;
                    highestSpeedAxis = "Z";
                }

                // Update running maximum speed
                if (currentOrientationChangeSpeed > runningMaximumSpeed)
                {
                    runningMaximumSpeed = currentOrientationChangeSpeed;
                }

                // Check if the current orientation change speed exceeds the threshold
                if (currentOrientationChangeSpeed > orientationChangeSpeedThreshold)
                {
                    OrientationRotationMaxed();
                }

                // Update previous orientation
                previousOrientation = orientation;
            }

            if (mirror)
            {
                transform.rotation = orientation;
            }
        }

        void OrientationRotationMaxed()
        {
            onHighOrientationChange.Invoke();
            Debug.Log("High orientation change detected");
        }
    }
}
