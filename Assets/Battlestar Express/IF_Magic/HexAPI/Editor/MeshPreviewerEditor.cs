using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshPreviewer))]
public class MeshPreviewerEditor : Editor
{
    private bool isDragging = false;
    private Vector2 dragStartPos;
    private float rotationX = 0f;
    private float rotationY = 0f;

    private static readonly int PreviewSize = 256;

    public override void OnInspectorGUI()
    {
        MeshPreviewer previewer = (MeshPreviewer)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Mesh Preview", EditorStyles.boldLabel);

        Rect rect = GUILayoutUtility.GetRect(PreviewSize, PreviewSize, GUILayout.ExpandWidth(true));

        if (previewer.mesh != null && previewer.material != null)
        {
            EditorGUI.DrawRect(rect, Color.white);

            Event e = Event.current;
            if (rect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    isDragging = true;
                    dragStartPos = e.mousePosition;
                    e.Use();
                }
                else if (e.type == EventType.MouseUp && e.button == 0)
                {
                    isDragging = false;
                    e.Use();
                }
                else if (e.type == EventType.MouseDrag && isDragging)
                {
                    Vector2 delta = e.mousePosition - dragStartPos;
                    rotationX += delta.y * 0.5f;
                    rotationY -= delta.x * 0.5f;
                    dragStartPos = e.mousePosition;
                    e.Use();
                }
            }

            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

            if (Event.current.type == EventType.Repaint)
            {
                RenderMeshPreview(previewer, rect, rotation);
            }
        }
        else
        {
            EditorGUI.HelpBox(rect, "No mesh or material assigned.", MessageType.Warning);
        }
    }

    private void RenderMeshPreview(MeshPreviewer previewer, Rect rect, Quaternion rotation)
    {
        if (Camera.current == null)
            return;

        Camera camera = Camera.current;
        RenderTexture renderTexture = new RenderTexture((int)rect.width, (int)rect.height, 16);

        camera.targetTexture = renderTexture;
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = Color.white;
        camera.orthographic = true;
        camera.orthographicSize = 2;
        camera.transform.position = rotation * new Vector3(0, 0, -5);
        camera.transform.rotation = rotation;

        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.white);

        previewer.material.SetPass(0);
        Graphics.DrawMeshNow(previewer.mesh, Matrix4x4.identity);

        camera.Render();

        GUI.DrawTexture(rect, renderTexture);

        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(renderTexture);
    }
}
