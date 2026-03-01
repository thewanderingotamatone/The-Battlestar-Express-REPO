using UnityEngine;
using UnityEditor;

namespace Magic
{
    public class DeviceWriter : MonoBehaviour
    {
        public string commandToSend = "";

        private Stream streamComponent;

        void Start()
        {
            // Find the Stream component in the scene
            streamComponent = FindObjectOfType<Stream>();

            if (streamComponent == null)
            {
                Debug.LogError("Stream component not found in the scene!");
            }
        }

        // This method will be called when the inspector button is clicked
        public void SendCommandToDevice()
        {
            if (streamComponent != null && !string.IsNullOrEmpty(commandToSend))
            {
                streamComponent.Output(commandToSend);
                Debug.Log("Command sent: " + commandToSend);
            }
            else
            {
                Debug.LogWarning("Stream component not found or command is empty!");
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DeviceWriter))]
    public class DeviceWriterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DeviceWriter deviceWriter = (DeviceWriter)target;

            if (GUILayout.Button("Send Command to Device"))
            {
                deviceWriter.SendCommandToDevice();
            }
        }
    }
#endif
}
