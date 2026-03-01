using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Magic.Algorithms
{
    [ExecuteAlways]
    public class OrientationMirror : MonoBehaviour
    {
        private Orientation targetOrientation;
        private Quaternion calibrationOffset = Quaternion.identity;
        private bool isCalibrated = false;

        private void Start()
        {
            // Find the Orientation script anywhere in the scene
            targetOrientation = FindObjectOfType<Orientation>();
            if (targetOrientation == null)
            {
                Debug.LogError("Orientation script not found in the scene.");
            }
        }

        private void Update()
        {
            if (targetOrientation != null)
            {
                if (isCalibrated)
                {
                    // Apply the calibration offset to zero out the local rotation
                    transform.localRotation = calibrationOffset * targetOrientation.orientation;
                }
                else
                {
                    transform.localRotation = targetOrientation.orientation;
                }
            }
        }

        [ContextMenu("Recalibrate")]
        public void Recalibrate()
        {
            if (targetOrientation != null)
            {
                // Calculate the calibration offset
                calibrationOffset = Quaternion.Inverse(targetOrientation.orientation);
                isCalibrated = true;
                Debug.Log("Recalibrated. Calibration offset set to: " + calibrationOffset);
            }
        }

        // Public method to be called from other scripts
        public void RecalibrateFromScript()
        {
            Recalibrate();
        }
    }

    // Custom inspector to add the Recalibrate button
    //[CustomEditor(typeof(OrientationMirror))]
    #if UNITY_EDITOR
    public class OrientationMirrorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            OrientationMirror myScript = (OrientationMirror)target;
            if (GUILayout.Button("Recalibrate"))
            {
                myScript.Recalibrate();
            }
        }
    }
    #endif
}
