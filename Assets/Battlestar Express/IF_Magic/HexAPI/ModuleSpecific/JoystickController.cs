using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magic.Modules
{
    public class JoystickController : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("Maximum rotation angle on the x and z axes.")]
        public float maxRotationAngle = 40f; // Max rotation angle, changeable in the inspector

        [Header("Target Object")]
        [Tooltip("The object that will be rotated based on joystick input.")]
        public Transform objectToRotate; // Object to rotate, settable in the inspector

        private JoystickModule joystickModule;
        private bool joystickActive = false;
        private Quaternion lastRotation;
        private float rotationVelocity;
        private const int sampleSize = 20;
        private Queue<float> velocitySamples = new Queue<float>(sampleSize);

        void Start()
        {
            // Find the JoystickModule in the scene
            joystickModule = FindObjectOfType<JoystickModule>();
            if (joystickModule != null)
            {
                joystickActive = joystickModule.gameObject.activeInHierarchy;
            }

            // Ensure there is an object to rotate
            if (objectToRotate == null)
            {
                Debug.LogError("No object set to rotate. Please assign an object to the 'objectToRotate' field in the inspector.");
            }

            lastRotation = objectToRotate.rotation; // Initialize lastRotation
        }

        void Update()
        {
            if (objectToRotate == null) return;

            float xRotation = 0f;
            float zRotation = 0f;

            if (joystickActive)
            {
                // Get joystick data
                float xInput = (joystickModule.x - 45f) / 55f; // Normalized between -1 and 1
                float zInput = (joystickModule.y - 45f) / 55f; // Normalized between -1 and 1

                // Calculate rotation angles and invert them
                xRotation = -xInput * maxRotationAngle;
                zRotation = -zInput * maxRotationAngle * -1f;
            }
            else
            {
                // Get WASD input as fallback
                float xInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow keys
                float zInput = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow keys

                // Calculate rotation angles and invert them
                xRotation = -xInput * maxRotationAngle;
                zRotation = -zInput * maxRotationAngle;
            }

            // Apply rotation
            objectToRotate.rotation = Quaternion.Euler(xRotation, 0, zRotation);

            // Calculate velocity of change in rotation
            rotationVelocity = Quaternion.Angle(lastRotation, objectToRotate.rotation) / Time.deltaTime;
            lastRotation = objectToRotate.rotation;

            // Store the velocity sample
            velocitySamples.Enqueue(rotationVelocity);
            if (velocitySamples.Count > sampleSize)
            {
                velocitySamples.Dequeue();
            }

            // Calculate average velocity
            float averageVelocity = 0f;
            foreach (float sample in velocitySamples)
            {
                averageVelocity += sample;
            }
            averageVelocity /= velocitySamples.Count;
        }

        public float GetAverageVelocity()
        {
            float averageVelocity = 0f;
            foreach (float sample in velocitySamples)
            {
                averageVelocity += sample;
            }
            return averageVelocity / velocitySamples.Count;
        }

        public float GetMaxRotationAngle()
        {
            return Mathf.Max(Mathf.Abs(objectToRotate.eulerAngles.x), Mathf.Abs(objectToRotate.eulerAngles.z));
        }
    }
}
